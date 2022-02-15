using Marten.Events;
using Marten.Schema;
using Marten.Schema.Indexing.Unique;
using MartenApi.EventStore.Events;

namespace MartenApi.EventStore.Projections.Document;

public record DocumentKeymap(
    [property: UniqueIndex(IndexType = UniqueIndexType.DuplicatedField, TenancyScope = TenancyScope.PerTenant)]
    DocumentId DocumentId,
    [property: Identity] string StreamKey)
{
    // TODO: Handle deletions / stream change
    public static DocumentKeymap Create(DocumentCreated @event, IEvent metadata)
    {
        return new DocumentKeymap(@event.DocumentId, metadata.StreamKey!);
    }
}