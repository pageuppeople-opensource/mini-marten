using Marten.Events.Projections;
using Marten.Storage;
using MartenApi.EventStore.Impl.Id;
using MartenApi.EventStore.Projections.Document;

namespace MartenApi.EventStore;

public static class MartenConfiguration
{
    public static void ConfigureProjectionsForEventStore(this ProjectionOptions projections)
    {
        projections.SelfAggregate<DocumentKeymap>(ProjectionLifecycle.Inline);
        projections.SelfAggregate<DocumentDetail>(ProjectionLifecycle.Live);
        projections.Add<DocumentOwnerProjection>(ProjectionLifecycle.Async);
        projections.SelfAggregate<DocumentSearch>(ProjectionLifecycle.Async);
    }

    public static void ConfigureEntityIdsForEventStore(this StorageFeatures storage)
    {
        storage.Add<EntityId<DocumentId>>();
        storage.Add<EntityId<UserId>>();
    }
}