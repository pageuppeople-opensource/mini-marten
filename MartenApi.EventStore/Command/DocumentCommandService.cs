using MartenApi.EventStore.Events;
using MartenApi.EventStore.Impl;
using MartenApi.EventStore.Impl.Id;
using MartenApi.EventStore.Projections.Document;

namespace MartenApi.EventStore.Command;

public class DocumentCommandService : IDocumentCommandService
{
    private readonly IIdProvider _idProvider;

    public DocumentCommandService(IIdProvider idProvider)
    {
        _idProvider = idProvider;
    }

    public async Task<DocumentDetail> CreateDocument(ITransactionSession session, string owner, string title,
        string content,
        CancellationToken token = default)
    {
        var newDocumentId = await _idProvider.GetNextId<DocumentId>(session.QuerySession, token);
        var streamKey = _idProvider.GenerateStreamKey();
        var createEvent = new DocumentCreated(new DocumentId(newDocumentId), owner, title, content);

        session.Events.StartStream<DocumentDetail>(streamKey, createEvent);
        return DocumentDetail.Create(createEvent, streamKey);
    }

    public async Task<DocumentDetail> UpdateDocument(ITransactionSession session, DocumentDetail documentDetail,
        string title, string content, CancellationToken token = default)
    {
        session.CorrelateEvents();

        var newDoc = documentDetail;

        if (documentDetail.Title != title)
        {
            var updateEvent = new DocumentTitleUpdated(documentDetail.DocumentId, title);
            await session.Events.AppendOptimistic(documentDetail.StreamKey, token, updateEvent);
            newDoc = newDoc.Apply(updateEvent, newDoc);
        }

        if (documentDetail.Content != content)
        {
            var updateEvent = new DocumentContentUpdated(documentDetail.DocumentId, content);
            await session.Events.AppendOptimistic(documentDetail.StreamKey, token, updateEvent);
            newDoc = newDoc.Apply(updateEvent, newDoc);
        }

        return newDoc;
    }

    public async Task<DocumentDetail> UpdateDocumentOwner(ITransactionSession session,
        DocumentDetail documentDetail,
        string newOwner,
        CancellationToken token = default)
    {
        var changeEvent = new DocumentOwnerChanged(documentDetail.DocumentId, documentDetail.Owner, newOwner);
        await session.Events.AppendOptimistic(documentDetail.StreamKey, token, changeEvent);

        return documentDetail.Apply(changeEvent, documentDetail);
    }
}