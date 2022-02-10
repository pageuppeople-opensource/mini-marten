using Marten;
using Weasel.Core;
using Weasel.Core.Migrations;
using Weasel.Postgresql;

namespace MartenApi.EventStore.Impl;

public abstract class EntityId : FeatureSchemaBase
{
    private readonly string _schema;

    protected EntityId(StoreOptions options, string entityType) : base($"{nameof(EntityId)}__{entityType}",
        options.Advanced.Migrator)
    {
        _schema = options.DatabaseSchemaName;
    }

    protected override IEnumerable<ISchemaObject> schemaObjects()
    {
        yield return new Sequence(new DbObjectName(_schema, $"sq_{Identifier}".ToLowerInvariant()), 1L);
    }
}

public class EntityId<TEntity> : EntityId
{
    public EntityId(StoreOptions options) : base(options, typeof(TEntity).Name)
    {
    }
}

public interface IEntityIdProvider
{
    Task<long> GetNextId<TEntity>(IQuerySession querySession, CancellationToken token);
}

public class EntityIdProvider : IEntityIdProvider
{
    // Note: This is deliberately not the interface - we need a property that is not on the interface.
    private readonly IDocumentStore _documentStore;

    public EntityIdProvider(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }


    public async Task<long> GetNextId<TEntity>(IQuerySession querySession, CancellationToken token)
    {
        var sequence = (_documentStore as DocumentStore)!.Storage.FindFeature(typeof(EntityId<TEntity>)).Objects
            .OfType<Sequence>().Single();

        return (await querySession.QueryAsync<long>(@"select nextval(?)", token, sequence.Identifier.QualifiedName))
            .Single();
    }
}