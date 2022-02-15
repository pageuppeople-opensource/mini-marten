using Marten;

namespace MartenApi.EventStore.Impl.Id;

public interface IIdProvider
{
    string GenerateStreamKey();
    Task<long> GetNextId<TEntity>(IQuerySession querySession, CancellationToken token);
}