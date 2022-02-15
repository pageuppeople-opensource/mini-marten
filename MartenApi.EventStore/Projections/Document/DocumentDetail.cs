using Marten.Events;
using Marten.Events.CodeGeneration;
using Marten.Schema;
using MartenApi.EventStore.Events;

namespace MartenApi.EventStore.Projections.Document;

public record DocumentDetail(
    DocumentId DocumentId,
    [property: Identity] string StreamKey,
    string Owner,
    string Title,
    string Content)
{
    public static DocumentDetail Create(DocumentCreated @event, IEvent metadata)
    {
        return Create(@event, metadata.StreamKey!);
    }

    [MartenIgnore]
    public static DocumentDetail Create(DocumentCreated @event, string streamKey)
    {
        return new DocumentDetail(
            @event.DocumentId,
            streamKey,
            @event.Owner,
            @event.Title,
            @event.Content);
    }

    public DocumentDetail Apply(DocumentTitleUpdated @event, DocumentDetail current)
    {
        return current with {Title = @event.Title};
    }

    public DocumentDetail Apply(DocumentContentUpdated @event, DocumentDetail current)
    {
        return current with {Content = @event.Content};
    }

    public DocumentDetail Apply(DocumentOwnerChanged @event, DocumentDetail current)
    {
        return current with {Owner = @event.NewOwner};
    }
}