using MartenApi.EventStore;
using MartenApi.EventStore.Document;
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

    /// <summary>
    /// Get a document by id.
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

        var document = await _documentService.TryGetDocumentById(session, unhashedId, token);
        if (document is null)
        {
            return NotFound(documentId);
        }

        // TODO: Validate user has access

        return Ok(new DocumentResponse(document));
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Post([FromBody] string content, CancellationToken token)
    {
        // TODO: Use auth for current owner
        var owner = "foo";
        // TODO: Validation

        return await _martenSessionFactory.RunInTransaction(async session =>
        {
            var document = await _documentService.CreateDocument(session, owner, content, token);

            await session.Commit(token);
            return CreatedAtAction(nameof(Get), new {documentId = document.DocumentId.Hash<Document>()}, new DocumentResponse(document));
        });
    }

    /// <summary>
    /// Update a document's content.
    /// </summary>
    /// <param name="documentId">The document id</param>
    /// <param name="newContent">The new content of the document</param>
    /// <param name="token">The cancellation token</param>
    /// <returns>The updated document</returns>
    [HttpPut("{documentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<ActionResult<DocumentResponse>> Put(string documentId, [FromBody] string newContent, CancellationToken token)
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

            var updatedDocument = await _documentService.UpdateDocumentContent(session, document, newContent, token);

            await session.Commit(token);
            return Ok(updatedDocument);
        });
    }

    public record DocumentResponse
    {
        public DocumentResponse(Document document)
        {
            DocumentId = document.DocumentId.Hash<Document>();
            Owner = document.Owner;
            Content = document.Content;
        }

        public string DocumentId { get; }
        public string Owner { get; }
        public string Content { get; }
    }
}