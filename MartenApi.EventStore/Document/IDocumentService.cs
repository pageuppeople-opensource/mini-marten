using Marten;
using MartenApi.EventStore.Document.Projections;
using MartenApi.EventStore.Impl;

namespace MartenApi.EventStore.Document;

public interface IDocumentService
{
    Task<Projections.Document?> TryGetDocumentById(IQuerySession querySession, long documentId,
        CancellationToken token = default);

    IAsyncEnumerable<DocumentSearch> SearchDocuments(IQuerySession querySession, string? searchQuery, int page = 0,
        int pageSize = 10, CancellationToken token = default);

    Task<Projections.Document?> TryGetDocumentByStreamKey(IQuerySession querySession, string streamKey,
        CancellationToken token = default);

    Task<string?> TryGetDocumentStreamKeyById(IQuerySession querySession, long documentId,
        CancellationToken token = default);

    Task<Projections.Document> CreateDocument(ITransactionSession session, string owner, string title,
        string content,
        CancellationToken token = default);

    Task<Projections.Document> UpdateDocument(ITransactionSession session, Projections.Document document,
        string title, string content,
        CancellationToken token = default);

    Task<Projections.Document> UpdateDocumentOwner(ITransactionSession session, Projections.Document document,
        string newOwner,
        CancellationToken token = default);

    IAsyncEnumerable<DocumentOwner> GetDocumentOwners(IQuerySession querySession,
        CancellationToken token = default);
}