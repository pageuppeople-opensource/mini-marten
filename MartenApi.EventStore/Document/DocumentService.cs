using Marten;
using Marten.Schema.Identity;
using MartenApi.EventStore.Document.Projections;
using MartenApi.EventStore.Impl;

namespace MartenApi.EventStore.Document;

public class DocumentService : IDocumentService
{
    private readonly IEntityIdProvider _entityIdProvider;

    public DocumentService(IEntityIdProvider entityIdProvider)
    {
        _entityIdProvider = entityIdProvider;
    }

    public async Task<Projections.Document?> TryGetDocumentById(IQuerySession querySession, long documentId,
        CancellationToken token = default)
    {
        var streamKey = await TryGetDocumentStreamKeyById(querySession, documentId, token);
        if (streamKey is null)
        {
            return null;
        }

        return await TryGetDocumentByStreamKey(querySession, streamKey, token);
    }

    public async Task<Projections.Document?> TryGetDocumentByStreamKey(IQuerySession querySession, string streamKey,
        CancellationToken token = default)
    {
        return await querySession.Events.AggregateStreamAsync<Projections.Document>(streamKey, token: token);
    }

    public async Task<string?> TryGetDocumentStreamKeyById(IQuerySession querySession, long documentId,
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

    public async Task<Projections.Document> CreateDocument(IEventTransactionSession session, string owner, string content,
        CancellationToken token = default)
    {
        var newDocumentId = await _entityIdProvider.GetNextId<Projections.Document>(session.QuerySession, token);
        var streamKey = CombGuidIdGeneration.NewGuid().ToString();
        var createEvent = new DocumentCreated(newDocumentId, Owner: owner, Title: string.Empty, Content: content);
        
        session.StartStream<Projections.Document>(streamKey, createEvent);
        return Projections.Document.Create(createEvent, streamKey);
    }

    public async Task<Projections.Document> UpdateDocumentContent(IEventTransactionSession session, Projections.Document document,
        string content, CancellationToken token = default)
    {
        var updateEvent = new DocumentContentUpdated(document.DocumentId, content);
        await session.AppendOptimistic(document.StreamKey, token, updateEvent);

        return Projections.Document.Apply(updateEvent, document);
    }

    public async Task<Projections.Document> UpdateDocumentOwner(IEventTransactionSession session, Projections.Document document,
        string newOwner,
        CancellationToken token = default)
    {
        var changeEvent = new DocumentOwnerChanged(document.DocumentId, document.Owner, newOwner);
        await session.AppendOptimistic(document.StreamKey, token, changeEvent);

        return Projections.Document.Apply(changeEvent, document);
    }
}