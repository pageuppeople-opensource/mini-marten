using Marten;
using Marten.Schema.Identity;
using MartenApi.EventStore.Impl;

namespace MartenApi.EventStore.Document;

public class DocumentService : IDocumentService
{
    public Task<Document?> TryGetDocumentById(IQuerySession querySession, string documentId,
        CancellationToken token = default)
    {
        return querySession.Events.AggregateStreamAsync<Document>(documentId, token: token);
    }

    public Document CreateDocument(IEventTransactionSession session, string owner, string content)
    {
        var streamId = CombGuidIdGeneration.NewGuid().ToString();
        var createEvent = new CreateDoc(streamId, Content: content, Owner: owner);

        session.StartStream<Document>(streamId, createEvent);

        return DocumentProjection.Instance.Create(createEvent);
    }

    public async Task<Document> UpdateDocumentContent(IEventTransactionSession session, Document document,
        string content, CancellationToken token = default)
    {
        var updateEvent = new UpdateDoc(document.DocumentId, content);
        await session.AppendOptimistic(document.DocumentId, token, updateEvent);

        return DocumentProjection.Instance.Apply(updateEvent, document);
    }

    public async Task<Document> UpdateDocumentOwner(IEventTransactionSession session, Document document,
        string newOwner,
        CancellationToken token = default)
    {
        var changeEvent = new ChangeDocOwner(document.DocumentId, document.Owner, newOwner);

        await session.AppendOptimistic(document.DocumentId, token, changeEvent);

        return DocumentProjection.Instance.Apply(changeEvent, document);
    }
}