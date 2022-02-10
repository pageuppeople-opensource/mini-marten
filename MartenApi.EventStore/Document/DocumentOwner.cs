using Marten.Events.Projections;
using Marten.Schema;

namespace MartenApi.EventStore.Document;

public record DocumentOwner([property: Identity] string Owner, IReadOnlyList<long>? DocumentIds = null)
{
    public IReadOnlyList<long> DocumentIds { get; init; } = DocumentIds ?? Array.Empty<long>();
}

public class DocumentOwnerProjection : ViewProjection<DocumentOwner, string>
{
    public DocumentOwnerProjection()
    {
        Identity<CreateDoc>(e => e.Owner);
        Identities<ChangeDocOwner>(e => new[] {e.OldOwner, e.NewOwner});
    }

    public DocumentOwner Create(CreateDoc @event)
    {
        return new DocumentOwner(@event.Owner, new[] {@event.DocumentId});
    }

    public DocumentOwner Create(ChangeDocOwner @event)
    {
        // ASSUMPTION: OldOwner is not the new owner id
        return new DocumentOwner(@event.NewOwner, new[] {@event.DocumentId});
    }

    // These never get called, the create always gets called.
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
            return current with {DocumentIds = current.DocumentIds.Where(d => d != @event.DocumentId).ToArray()};
        }

        return current with
        {
            DocumentIds = current.DocumentIds.Concat(new[] {@event.DocumentId}).Distinct().ToArray()
        };
    }
}