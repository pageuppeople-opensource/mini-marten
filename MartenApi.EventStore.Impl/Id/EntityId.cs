using Marten;
using Weasel.Core;
using Weasel.Core.Migrations;
using Weasel.Postgresql;

namespace MartenApi.EventStore.Impl.Id;

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