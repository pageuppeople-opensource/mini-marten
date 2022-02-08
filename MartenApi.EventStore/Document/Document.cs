using Marten.Events.Aggregation;

namespace MartenApi.EventStore.Document;

public record Document(string DocumentId, string Owner, string Content);

public class DocumentProjection : AggregateProjection<Document>
{
    public static DocumentProjection Instance { get; } = new();

    public Document Create(CreateDoc @event)
    {
        return new Document(DocumentId: @event.DocumentId, Owner: @event.Owner, Content: @event.Content);
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