using Marten;
using Marten.Schema.Identity;
using MartenApi.EventStore.Impl;

namespace MartenApi.EventStore.Document;

public class DocumentService : IDocumentService
{
    private readonly IEntityIdProvider _entityIdProvider;

    public DocumentService(IEntityIdProvider entityIdProvider)
    {
        _entityIdProvider = entityIdProvider;
    }

    public async Task<Document?> TryGetDocumentById(IQuerySession querySession, long documentId,
        CancellationToken token = default)
    {
        var streamKey = await TryGetDocumentStreamKeyById(querySession, documentId, token);

        if (streamKey is null)
        {
            return null;
        }

        return await TryGetDocumentByStreamKey(querySession, streamKey, token);
    }

    public async Task<Document?> TryGetDocumentByStreamKey(IQuerySession querySession, string streamKey,
        CancellationToken token = default)
    {
        return await querySession.Events.AggregateStreamAsync<Document>(streamKey, token: token);
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

    public async Task<Document> CreateDocument(IEventTransactionSession session, string owner, string content,
        CancellationToken token = default)
    {
        var newDocumentId = await _entityIdProvider.GetNextId<Document>(session.QuerySession, token);
        var streamKey = CombGuidIdGeneration.NewGuid().ToString();
        var createEvent = new CreateDoc(newDocumentId, Content: content, Owner: owner);
        
        session.StartStream<Document>(streamKey, createEvent);
        return DocumentProjection.Instance.Create(createEvent, streamKey);
    }

    public async Task<Document> UpdateDocumentContent(IEventTransactionSession session, Document document,
        string content, CancellationToken token = default)
    {
        var updateEvent = new UpdateDoc(document.DocumentId, content);
        await session.AppendOptimistic(document.StreamKey, token, updateEvent);

        return DocumentProjection.Instance.Apply(updateEvent, document);
    }

    public async Task<Document> UpdateDocumentOwner(IEventTransactionSession session, Document document,
        string newOwner,
        CancellationToken token = default)
    {
        var changeEvent = new ChangeDocOwner(document.DocumentId, document.Owner, newOwner);

        await session.AppendOptimistic(document.StreamKey, token, changeEvent);

        return DocumentProjection.Instance.Apply(changeEvent, document);
    }
}