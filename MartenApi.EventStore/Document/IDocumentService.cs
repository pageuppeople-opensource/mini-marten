using Marten;
using MartenApi.EventStore.Impl;

namespace MartenApi.EventStore.Document;

public interface IDocumentService
{
    Task<Document?> TryGetDocumentById(IQuerySession querySession, long documentId,
        CancellationToken token = default);

    Task<Document?> TryGetDocumentByStreamKey(IQuerySession querySession, string streamKey,
        CancellationToken token = default);

    Task<string?> TryGetDocumentStreamKeyById(IQuerySession querySession, long documentId,
        CancellationToken token = default);

    Task<Document> CreateDocument(IEventTransactionSession session, string owner, string content,
        CancellationToken token = default);

    Task<Document> UpdateDocumentContent(IEventTransactionSession session, Document document, string content,
        CancellationToken token = default);

    Task<Document> UpdateDocumentOwner(IEventTransactionSession session, Document document, string newOwner,
        CancellationToken token = default);

    IAsyncEnumerable<DocumentOwner> GetDocumentOwners(IQuerySession querySession,
        CancellationToken token = default);
}