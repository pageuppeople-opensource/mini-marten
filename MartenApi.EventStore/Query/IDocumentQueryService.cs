using Marten;
using MartenApi.EventStore.Projections.Document;

namespace MartenApi.EventStore.Query;

public interface IDocumentQueryService
{
    Task<DocumentDetail?> TryGetDocumentById(IQuerySession querySession, DocumentId documentId,
        CancellationToken token = default);

    IAsyncEnumerable<DocumentSearch> SearchDocuments(IQuerySession querySession, string? searchQuery, int page = 0,
        int pageSize = 10, CancellationToken token = default);

    Task<DocumentDetail?> TryGetDocumentByStreamKey(IQuerySession querySession, string streamKey,
        CancellationToken token = default);

    Task<string?> TryGetDocumentStreamKeyById(IQuerySession querySession, DocumentId documentId,
        CancellationToken token = default);
}