using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Events.CodeGeneration;
using MartenApi.EventStore.Document.Events;

namespace MartenApi.EventStore.Document;

public class DocumentAggregation : AggregateProjection<Document>
{
    public static DocumentAggregation Instance { get; } = new ();

    public Document Create(CreateDoc @event, IEvent metadata)
    {
        return Create(@event, metadata.StreamId);
    }

    [MartenIgnore]
    public Document Create(CreateDoc @event, Guid streamId)
    {
        return new Document
        {
            Id = streamId,
            Owner = @event.Owner,
            Content = @event.Content
        };
    }

    public Document Apply(UpdateDoc @event, Document current)
    {
        return current with {Content = @event.Content};
    }

    public Document Apply(ChangeDocOwner @event, Document current)
    {
        return current with {Owner = @event.NewOwner};
    }
}