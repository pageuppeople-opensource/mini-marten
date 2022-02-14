using System.Runtime.CompilerServices;
using MartenApi.EventStore.Document;
using MartenApi.EventStore.Document.Projections;
using MartenApi.EventStore.Impl;
using Microsoft.AspNetCore.Mvc;

namespace MartenApi.Controllers;

[Route("Document")]
[ApiController]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly IMartenSessionFactory _martenSessionFactory;

    public DocumentController(IMartenSessionFactory martenSessionFactory, IDocumentService documentService)
    {
        _martenSessionFactory = martenSessionFactory;
        _documentService = documentService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async IAsyncEnumerable<DocumentSearchResponse> Search(string? searchText,
        [EnumeratorCancellation] CancellationToken token,
        int page = 0)
    {
        await using var session = _martenSessionFactory.GetQuerySession();

        var documents = _documentService.SearchDocuments(session, searchText, page, 10, token);

        await foreach (var document in documents.WithCancellation(token))
            yield return new DocumentSearchResponse(document);
    }

    /// <summary>
    ///     Get a document by id.
    /// </summary>
    /// <param name="documentId">The document id</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>The current version of the document</returns>
    /// <response code="200">Returns the newly created item</response>
    /// <response code="404">The document does not exist or you do not have access to it</response>
    [HttpGet("{documentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<ActionResult<DocumentResponse>> Get(string documentId, CancellationToken token)
    {
        await using var session = _martenSessionFactory.GetQuerySession();

        if (!documentId.TryUnHash<Document>(out var unhashedId))
        {
            return NotFound();
        }

        // TODO: Validate user has access
        var document = await _documentService.TryGetDocumentById(session, unhashedId, token);
        if (document is null)
        {
            return NotFound(documentId);
        }

        return Ok(new DocumentResponse(document));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentRequest request, CancellationToken token)
    {
        // TODO: Use auth for current owner
        var owner = "foo";
        // TODO: Validation

        return await _martenSessionFactory.RunInTransaction(async session =>
        {
            var document = await _documentService.CreateDocument(session, owner, request.Title, request.Content, token);

            await session.Commit(token);
            return CreatedAtAction(nameof(Get), new {documentId = document.DocumentId.Hash<Document>()},
                new DocumentResponse(document));
        });
    }

    /// <summary>
    ///     Update a document's content.
    /// </summary>
    /// <param name="documentId">The document id</param>
    /// <param name="newContent">The new content of the document</param>
    /// <param name="token">The cancellation token</param>
    /// <returns>The updated document</returns>
    [HttpPut("{documentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<ActionResult<DocumentResponse>> UpdateDocument(string documentId,
        [FromBody] UpdateDocumentRequest newContent, CancellationToken token)
    {
        return await _martenSessionFactory.RunInTransaction<ActionResult<DocumentResponse>>(async session =>
        {
            // Forces all changes after this line to be committed immediately after the latest event version _as of this line_
            // Or rollback
            if (!documentId.TryUnHash<Document>(out var unhashedId))
            {
                return NotFound(documentId);
            }

            var streamKey = await _documentService.TryGetDocumentStreamKeyById(session.QuerySession, unhashedId, token);
            if (streamKey is null)
            {
                return NotFound(documentId);
            }

            if (!await session.MarkStreamForUpdateIfExists(streamKey, token))
            {
                return NotFound(documentId);
            }

            var document = await _documentService.TryGetDocumentByStreamKey(session.QuerySession, streamKey, token);
            if (document is null)
            {
                return NotFound(documentId);
            }

            var updatedDocument =
                await _documentService.UpdateDocument(
                    session,
                    document,
                    newContent.Title ?? document.Title,
                    newContent.Content ?? document.Content,
                    token);

            await session.Commit(token);
            return Ok(new DocumentResponse(updatedDocument));
        });
    }

    public record DocumentSearchResponse
    {
        public DocumentSearchResponse(DocumentSearch document)
        {
            DocumentId = document.DocumentId.Hash<Document>();
            Owner = document.Owner;
            Title = document.Title;
            LastModified = document.LastModified;
        }

        public string DocumentId { get; }
        public string Owner { get; }

        public string Title { get; }

        public DateTimeOffset LastModified { get; }
    }

    public record UpdateDocumentRequest
    {
        public string? Title { get; init; }
        public string? Content { get; init; }
    }

    public record CreateDocumentRequest
    {
        public string Title { get; init; }
        public string Content { get; init; }
    }

    public record DocumentResponse
    {
        public DocumentResponse(Document document)
        {
            DocumentId = document.DocumentId.Hash<Document>();
            Owner = document.Owner;
            Title = document.Title;
            Content = document.Content;
        }

        public string DocumentId { get; }
        public string Owner { get; }

        public string Title { get; }
        public string Content { get; }
    }
}