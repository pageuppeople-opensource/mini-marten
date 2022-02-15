using Marten;
using Marten.Schema.Identity;
using Weasel.Postgresql;

namespace MartenApi.EventStore.Impl.Id;

public class IdProvider : IIdProvider
{
    // Note: This is deliberately not the interface - we need a property that is not on the interface.
    private readonly DocumentStore _documentStore;

    public IdProvider(IDocumentStore documentStore)
    {
        _documentStore = (documentStore as DocumentStore)!;
    }

    public string GenerateStreamKey()
    {
        return CombGuidIdGeneration.NewGuid().ToString();
    }

    public async Task<long> GetNextId<TEntity>(IQuerySession querySession, CancellationToken token)
    {
        var sequence = _documentStore.Storage.FindFeature(typeof(EntityId<TEntity>)).Objects
            .OfType<Sequence>().Single();

        return (await querySession.QueryAsync<long>(@"select nextval(?)", token, sequence.Identifier.QualifiedName))
            .Single();
    }
}