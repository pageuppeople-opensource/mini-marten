using Marten.Events.Projections;
using Marten.Schema;
using MartenApi.EventStore.Events;

namespace MartenApi.EventStore.Projections.Document;

public record DocumentOwner(
    [property: Identity] string Owner, 
    IReadOnlyList<DocumentId>? DocumentIds = null)
{
    public IReadOnlyList<DocumentId> DocumentIds { get; init; } = DocumentIds ?? Array.Empty<DocumentId>();
}

public class DocumentOwnerProjection : ViewProjection<DocumentOwner, string>
{
    public DocumentOwnerProjection()
    {
        Identity<DocumentCreated>(e => e.Owner);
        Identities<DocumentOwnerChanged>(e => new[] {e.OldOwner, e.NewOwner});
    }

    public DocumentOwner Create(DocumentCreated @event)
    {
        return new DocumentOwner(@event.Owner, new [] {@event.DocumentId});
    }

    public DocumentOwner Create(DocumentOwnerChanged @event)
    {
        // ASSUMPTION: OldOwner is not the new owner id
        return new DocumentOwner(@event.NewOwner, new [] {@event.DocumentId});
    }

    // These never get called, the create always gets called.
    public DocumentOwner Apply(DocumentCreated @event, DocumentOwner current)
    {
        return current with
        {
            Owner = @event.Owner,
            DocumentIds = current.DocumentIds.Concat(new[] {@event.DocumentId}).Distinct().ToArray()
        };
    }

    public DocumentOwner Apply(DocumentOwnerChanged @event, DocumentOwner current)
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