using MartenApi.EventStore.Document;
using MartenApi.EventStore.Impl;
using Microsoft.AspNetCore.Mvc;

namespace MartenApi.Controllers;

[Route("Document")]
[ApiController]
public class DocumentController : ControllerBase
{
    private readonly IMartenSessionFactory _martenSessionFactory;
    private readonly IDocumentService _documentService;

    public DocumentController(IMartenSessionFactory martenSessionFactory, IDocumentService documentService)
    {
        _martenSessionFactory = martenSessionFactory;
        _documentService = documentService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken token)
    {
        await using var session = _martenSessionFactory.GetQuerySession();

        var document = await _documentService.TryGetDocumentById(session, id.ToString(), token);
        if (document is null)
        {
            return NotFound(id);
        }

        // TODO: Validate user has access

        return Ok(document);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] string content, CancellationToken token)
    {
        // TODO: Use auth for current owner
        var owner = "foo";
        // TODO: Validation

        return await _martenSessionFactory.RunInTransaction(async session =>
        {
            var document = _documentService.CreateDocument(session, owner, content);

            await session.Commit(token);
            return CreatedAtAction(nameof(Get), new { id = document.DocumentId }, document);
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] string newContent, CancellationToken token)
    {
        // TODO: Validation

        return await _martenSessionFactory.RunInTransaction<IActionResult>(async session =>
        {
            // Forces all changes after this line to be committed immediately after the latest event version _as of this line_
            // Or rollback
            if (!await session.MarkStreamForUpdateIfExists(id.ToString(), token))
            {
                return NotFound(id);
            }

            var document = await _documentService.TryGetDocumentById(session.QuerySession, id.ToString(), token);
            if (document is null)
            {
                return NotFound(id);
            }

            var updatedDocument = await _documentService.UpdateDocumentContent(session, document, newContent, token);

            await session.Commit(token);
            return Ok(updatedDocument);
        });
    }
}