using Marten;
using MartenApi.EventStore.Document.Projections;
using MartenApi.EventStore.Impl;

namespace MartenApi.EventStore.Document;

public interface IDocumentService
{
    Task<Projections.Document?> TryGetDocumentById(IQuerySession querySession, long documentId,
        CancellationToken token = default);

    Task<Projections.Document?> TryGetDocumentByStreamKey(IQuerySession querySession, string streamKey,
        CancellationToken token = default);

    Task<string?> TryGetDocumentStreamKeyById(IQuerySession querySession, long documentId,
        CancellationToken token = default);

    Task<Projections.Document> CreateDocument(IEventTransactionSession session, string owner, string content,
        CancellationToken token = default);

    Task<Projections.Document> UpdateDocumentContent(IEventTransactionSession session, Projections.Document document, string content,
        CancellationToken token = default);

    Task<Projections.Document> UpdateDocumentOwner(IEventTransactionSession session, Projections.Document document, string newOwner,
        CancellationToken token = default);

    IAsyncEnumerable<DocumentOwner> GetDocumentOwners(IQuerySession querySession,
        CancellationToken token = default);
}