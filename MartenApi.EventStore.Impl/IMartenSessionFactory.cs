using Marten;

namespace MartenApi.EventStore.Impl;

public interface IMartenSessionFactory
{
    Task<TResult> RunInTransaction<TResult>(Func<IEventTransactionSession, Task<TResult>> command);

    Task RunInTransaction(Func<IEventTransactionSession, Task> command);

    IQuerySession GetQuerySession();
}