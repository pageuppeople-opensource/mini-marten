using Marten;
using Marten.Events;

namespace MartenApi.EventStore.Impl;

public interface IEventTransactionSession : IEventStore, IDisposable, IAsyncDisposable
{
    IQuerySession QuerySession { get; }
    Task<bool> MarkStreamForUpdateIfExists(string streamKey, CancellationToken cancellationToken = default);
    PendingChangesSummary PendingChanges();
    ValueTask Rollback();
    Task Commit(CancellationToken cancellationToken = default);
}