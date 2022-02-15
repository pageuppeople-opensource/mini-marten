using Marten;

namespace MartenApi.EventStore.Impl;

public interface IMartenSessionFactory
{
    Task<TResult> RunInTransaction<TResult>(Func<ITransactionSession, Task<TResult>> command, bool allowRetry = true);

    Task RunInTransaction(Func<ITransactionSession, Task> command, bool allowRetry = true);

    IQuerySession GetQuerySession();
}