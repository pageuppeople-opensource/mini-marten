using Marten;
using MartenApi.EventStore.Projections.Document;

namespace MartenApi.EventStore.Query;

public class DocumentQueryService : IDocumentQueryService
{
    public async Task<DocumentDetail?> TryGetDocumentById(IQuerySession querySession, DocumentId documentId,
        CancellationToken token = default)
    {
        var streamKey = await TryGetDocumentStreamKeyById(querySession, documentId, token);
        if (streamKey is null)
        {
            return null;
        }

        return await TryGetDocumentByStreamKey(querySession, streamKey, token);
    }

    public IAsyncEnumerable<DocumentSearch> SearchDocuments(IQuerySession querySession, string? searchQuery,
        int page = 0, int pageSize = 10,
        CancellationToken token = default)
    {
        if (page <= 1)
        {
            page = 1;
        }

        if (pageSize <= 0)
        {
            return AsyncEnumerable.Empty<DocumentSearch>();
        }

        var query = querySession.Query<DocumentSearch>() as IQueryable<DocumentSearch>;

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            query = query.Where(x => x.PlainTextSearch(searchQuery));
        }

        return query.Skip((page - 1) * pageSize).Take(pageSize)
            .ToAsyncEnumerable(token);
    }

    public async Task<DocumentDetail?> TryGetDocumentByStreamKey(IQuerySession querySession, string streamKey,
        CancellationToken token = default)
    {
        return await querySession.Events.AggregateStreamAsync<DocumentDetail>(streamKey, token: token);
    }

    public async Task<string?> TryGetDocumentStreamKeyById(IQuerySession querySession, DocumentId documentId,
        CancellationToken token = default)
    {
        var streamKey = await querySession.Query<DocumentKeymap>().Where(x => x.DocumentId == documentId)
            .Select(x => x.StreamKey).SingleOrDefaultAsync(token);

        return streamKey;
    }

    public IAsyncEnumerable<DocumentOwner> GetDocumentOwners(IQuerySession querySession,
        CancellationToken token = default)
    {
        return querySession.Query<DocumentOwner>().OrderBy(d => d.Owner).ToAsyncEnumerable(token);
    }
}