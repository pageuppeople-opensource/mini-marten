using Marten;
using MartenApi.EventStore.Document.Events;
using MartenApi.EventStore.Impl;

namespace MartenApi.EventStore.Document;

public class DocumentService : IDocumentService
{
    public Task<Document?> TryGetDocumentById(IQuerySession querySession, Guid documentId,
        CancellationToken token = default)
    {
        return querySession.Events.AggregateStreamAsync<Document>(documentId, token: token);
    }

    public Document CreateDocument(IEventTransactionSession session, string owner, string content)
    {
        var createEvent = new CreateDoc {Content = content, Owner = owner};
        var documentId = session.StartStream<Document>(createEvent).Id;
        return DocumentAggregation.Instance.Create(createEvent, documentId);
    }

    public async Task<Document> UpdateDocumentContent(IEventTransactionSession session, Document document,
        string content, CancellationToken token = default)
    {
        var updateEvent = new UpdateDoc
        {
            Content = content
        };

        await session.AppendOptimistic(document.Id, updateEvent);

        return DocumentAggregation.Instance.Apply(updateEvent, document);
    }

    public async Task<Document> UpdateDocumentOwner(IEventTransactionSession session, Document document, string owner,
        CancellationToken token = default)
    {
        var changeEvent = new ChangeDocOwner
        {
            NewOwner = owner
        };

        await session.AppendOptimistic(document.Id, changeEvent);

        return DocumentAggregation.Instance.Apply(changeEvent, document);
    }
}