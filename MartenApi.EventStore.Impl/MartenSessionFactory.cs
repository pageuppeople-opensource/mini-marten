using Baseline.Dates;
using Marten;
using Marten.Exceptions;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace MartenApi.EventStore.Impl;

public class MartenSessionFactory : IMartenSessionFactory
{
    private readonly IDocumentStore _documentStore;
    private readonly ILogger<MartenSessionFactory> _logger;
    private readonly string _tenantId;
    private readonly AsyncRetryPolicy _transactionRetryPolicy;

    public MartenSessionFactory(IDocumentStore documentStore /*, string tenantId*/,
        ILogger<MartenSessionFactory> logger)
    {
        // TODO: Get tenant id from service (auth)
        _documentStore = documentStore;
        _logger = logger;
        _tenantId = "FAKE TENANT ID";

        _transactionRetryPolicy = Policy.Handle<ConcurrencyException>().WaitAndRetryAsync(
            new[] {0.Seconds(), 500.Milliseconds(), 1.Seconds()},
            (ex, _, context) =>
            {
                _logger.LogWarning(ex,
                    $"Concurrency error occurred while writing to event store, will retry. Retry: {context.Count}");
            });
    }

    public IQuerySession GetQuerySession()
    {
        return _documentStore.QuerySession(_tenantId);
    }

    public async Task<TResult> RunInTransaction<TResult>(Func<ITransactionSession, Task<TResult>> command,
        bool allowRetry = true)
    {
        var result = await _transactionRetryPolicy.ExecuteAsync(async () =>
        {
            await using var session = new TransactionSession(CreateDocumentSession);
            var result = await command(session);

            var pendingChanges = session.PendingChanges();
            if (pendingChanges.AnyChanges)
            {
                throw new UncommittedChangesException(pendingChanges);
            }

            return result;
        });

        return result;
    }

    public async Task RunInTransaction(Func<ITransactionSession, Task> command, bool allowRetry = true)
    {
        await _transactionRetryPolicy.ExecuteAsync(async () =>
        {
            await using var session = new TransactionSession(CreateDocumentSession);
            await command(session);

            var pendingChanges = session.PendingChanges();
            if (pendingChanges.AnyChanges)
            {
                throw new UncommittedChangesException(pendingChanges);
            }
        });
    }

    private IDocumentSession CreateDocumentSession()
    {
        var session = _documentStore.LightweightSession(_tenantId);
        // TODO: Add metadata here (current user etc)
        // session.SetHeader("CurrentUser", _authService.CurrentUser);
        return session;
    }
}