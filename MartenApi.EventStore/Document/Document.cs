using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Events.CodeGeneration;
using Marten.Schema;

namespace MartenApi.EventStore.Document;

public record Document(long DocumentId, [property: Identity] string StreamKey, string Owner, string Content);

public record DocumentKeymap(long DocumentId, [property: Identity] string StreamKey);

public class DocumentProjection : AggregateProjection<Document>
{
    public static DocumentProjection Instance { get; } = new();

    public Document Create(CreateDoc @event, IEvent metadata)
    {
        return Create(@event, metadata.StreamKey!);
    }

    [MartenIgnore]
    public Document Create(CreateDoc @event, string streamKey)
    {
        return new Document(
            DocumentId: @event.DocumentId, 
            StreamKey: streamKey, 
            Owner: @event.Owner,
            Content: @event.Content);
    }

    public Document Apply(UpdateDoc @event, Document current)
    {
        return current with { Content = @event.Content };
    }

    public Document Apply(ChangeDocOwner @event, Document current)
    {
        return current with { Owner = @event.NewOwner };
    }
}

public class DocumentKeymapProjection : AggregateProjection<DocumentKeymap>
{
    public DocumentKeymap Create(CreateDoc @event, IEvent metadata)
    {
        return new DocumentKeymap(@event.DocumentId, metadata.StreamKey!);
    }

    // TODO: Handle deletions / stream change
}