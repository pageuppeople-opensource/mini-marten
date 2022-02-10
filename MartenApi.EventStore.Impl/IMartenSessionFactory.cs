using Marten;

namespace MartenApi.EventStore.Impl;

public interface IMartenSessionFactory
{
    Task<TResult> RunInTransaction<TResult>(Func<ITransactionSession, Task<TResult>> command);

    Task RunInTransaction(Func<ITransactionSession, Task> command);

    IQuerySession GetQuerySession();
}