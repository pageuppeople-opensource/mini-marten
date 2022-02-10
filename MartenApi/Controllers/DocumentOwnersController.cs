using System.Runtime.CompilerServices;
using MartenApi.EventStore.Document;
using MartenApi.EventStore.Impl;
using Microsoft.AspNetCore.Mvc;

namespace MartenApi.Controllers
{
    [Route("DocumentOwners")]
    [ApiController]
    public class DocumentOwnersController : ControllerBase
    {
        private readonly IMartenSessionFactory _martenSessionFactory;
        private readonly IDocumentService _documentService;

        public DocumentOwnersController(IMartenSessionFactory martenSessionFactory, IDocumentService documentService)
        {
            _martenSessionFactory = martenSessionFactory;
            _documentService = documentService;
        }

        [HttpGet]
        public async IAsyncEnumerable<DocumentOwner> ListByOwners([EnumeratorCancellation] CancellationToken token)
        {
            await using var session = _martenSessionFactory.GetQuerySession();
            await foreach (var documentOwner in _documentService.GetDocumentOwners(session, token))
            {
                yield return documentOwner;
            }
        }
    }
}
