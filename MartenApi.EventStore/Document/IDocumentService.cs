using Marten;
using Marten.Events;
using MartenApi.EventStore.Impl;

namespace MartenApi.EventStore.Document;

public interface IDocumentService
{
    Task<Document?> TryGetDocumentById(IQuerySession querySession, string documentId,
        CancellationToken token = default);

    Document CreateDocument(IEventTransactionSession session, string owner, string content);
    Task<Document> UpdateDocumentContent(IEventTransactionSession session, Document document, string content, CancellationToken token = default);
    Task<Document> UpdateDocumentOwner(IEventTransactionSession session, Document document, string owner, CancellationToken token = default);
}