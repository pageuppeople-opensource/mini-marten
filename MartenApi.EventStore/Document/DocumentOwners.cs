using Marten.Events.Projections;

namespace MartenApi.EventStore.Document;

public record DocumentOwner(string Owner, IReadOnlyList<string>? DocumentIds = null)
{
    public IReadOnlyList<string> DocumentIds { get; init; } = DocumentIds ?? Array.Empty<string>();
}

public class DocumentOwnerProjection : ViewProjection<DocumentOwner, string>
{
    public DocumentOwnerProjection()
    {
        Identity<CreateDoc>(e => e.Owner);
        Identities<ChangeDocOwner>(e => new [] { e.OldOwner, e.NewOwner });
    }



    public DocumentOwner Apply(CreateDoc @event, DocumentOwner current)
    {
        return current with
        {
            Owner = @event.Owner,
            DocumentIds = current.DocumentIds.Concat(new[] {@event.DocumentId}).Distinct().ToArray()
        };
    }

    public DocumentOwner Apply(ChangeDocOwner @event, DocumentOwner current)
    {
        if (@event.OldOwner == current.Owner)
        {
            return current with { DocumentIds = current.DocumentIds.Where(d => d != @event.DocumentId).ToArray()};
        }

        return current with
        {
            Owner = @event.NewOwner,
            DocumentIds = current.DocumentIds.Concat(new[] { @event.DocumentId }).Distinct().ToArray()
        };
    }
}