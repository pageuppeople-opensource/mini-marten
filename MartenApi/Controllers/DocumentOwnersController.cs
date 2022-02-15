using System.Runtime.CompilerServices;
using MartenApi.EventStore.Impl;
using MartenApi.EventStore.Projections.Document;
using MartenApi.EventStore.Query;
using Microsoft.AspNetCore.Mvc;

namespace MartenApi.Controllers;

[Route("DocumentOwners")]
[ApiController]
public class DocumentOwnersController : ControllerBase
{
    private readonly IMartenSessionFactory _martenSessionFactory;
    private readonly IDocumentQueryService _documentQueryService;

    public DocumentOwnersController(IMartenSessionFactory martenSessionFactory, IDocumentQueryService documentQueryService)
    {
        _martenSessionFactory = martenSessionFactory;
        _documentQueryService = documentQueryService;
    }

    [HttpGet]
    public async IAsyncEnumerable<DocumentOwner> ListByOwners([EnumeratorCancellation] CancellationToken token)
    {
        await using var session = _martenSessionFactory.GetQuerySession();
        await foreach (var documentOwner in _documentQueryService.GetDocumentOwners(session, token))
        {
            yield return documentOwner;
        }
    }
}