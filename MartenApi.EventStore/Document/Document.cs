using Marten.Events.Aggregation;
using Marten.Schema;

namespace MartenApi.EventStore.Document;

public record Document([property: Identity] string DocumentId, string? Owner, string? Content);

public class DocumentAggregation : AggregateProjection<Document>
{
    public static DocumentAggregation Instance { get; } = new();

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