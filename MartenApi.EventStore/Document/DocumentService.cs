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

        return DocumentAggregation.Instance.Create(createEvent);
    }

    public async Task<Document> UpdateDocumentContent(IEventTransactionSession session, Document document,
        string content, CancellationToken token = default)
    {
        var documentId = document.DocumentId ??
                         throw new ArgumentException("Document id is null", nameof(document.DocumentId));

        var updateEvent = new UpdateDoc(documentId, content);
        await session.AppendOptimistic(documentId, token, updateEvent);

        return DocumentAggregation.Instance.Apply(updateEvent, document);
    }

    public async Task<Document> UpdateDocumentOwner(IEventTransactionSession session, Document document, string owner,
        CancellationToken token = default)
    {
        var documentId = document.DocumentId ??
                         throw new ArgumentException("Document id is null", nameof(document.DocumentId));

        var changeEvent = new ChangeDocOwner(documentId, owner);

        await session.AppendOptimistic(documentId, token, changeEvent);

        return DocumentAggregation.Instance.Apply(changeEvent, document);
    }
}