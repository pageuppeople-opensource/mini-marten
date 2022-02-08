using Marten;

namespace MartenApi.EventStore.Impl;

public class MartenSessionFactory : IMartenSessionFactory
{
    private readonly IDocumentStore _documentStore;
    private readonly string _tenantId;

    public MartenSessionFactory(IDocumentStore documentStore/*, string tenantId*/)
    {
        // TODO: Get tenant id from service (auth)
        _documentStore = documentStore;
        _tenantId = "FAKE TENANT ID";
    }

    public async Task<TResult> RunInTransaction<TResult>(Func<IEventTransactionSession, Task<TResult>> command)
    {
        // TODO: Wrap in a try/catch to support event retry
        await using var session = new EventTransactionSession(CreateDocumentSession);
        var result = await command(session);

        var pendingChanges = session.PendingChanges();
        if (pendingChanges.AnyChanges)
        {
            throw new UncommittedChangesException(pendingChanges);
        }

        return result;
    }

    public async Task RunInTransaction(Func<IEventTransactionSession, Task> command)
    {
        // TODO: Wrap in a try/catch to support event retry
        await using var session = new EventTransactionSession(CreateDocumentSession);
        await command(session);

        var pendingChanges = session.PendingChanges();
        if (pendingChanges.AnyChanges)
        {
            throw new UncommittedChangesException(pendingChanges);
        }
    }

    public IQuerySession GetQuerySession()
    {
        return _documentStore.QuerySession(_tenantId);
    }

    private IDocumentSession CreateDocumentSession()
    {
        var session = _documentStore.LightweightSession(_tenantId);
        // TODO: Add metadata here (current user etc)
        // session.SetHeader("CurrentUser", _authService.CurrentUser);
        return session;
    }
}