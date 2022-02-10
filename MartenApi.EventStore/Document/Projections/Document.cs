using Marten.Events;
using Marten.Events.CodeGeneration;
using Marten.Schema;

namespace MartenApi.EventStore.Document.Projections;

public record Document(
    long DocumentId, 
    [property: Identity] string StreamKey, 
    string Owner, 
    string Title,
    string Content)
{
    public static Document Create(DocumentCreated @event, IEvent metadata)
    {
        return Create(@event, metadata.StreamKey!);
    }

    [MartenIgnore]
    public static Document Create(DocumentCreated @event, string streamKey)
    {
        return new Document(
            @event.DocumentId,
            streamKey,
            @event.Owner,
            @event.Title,
            @event.Content);
    }

    public Document Apply(DocumentTitleUpdated @event, Document current)
    {
        return current with { Title = @event.Title };
    }

    public Document Apply(DocumentContentUpdated @event, Document current)
    {
        return current with { Content = @event.Content };
    }

    public Document Apply(DocumentOwnerChanged @event, Document current)
    {
        return current with { Owner = @event.NewOwner };
    }
}