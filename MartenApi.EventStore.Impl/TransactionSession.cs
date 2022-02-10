using Marten;
using Marten.Events;
using Marten.Exceptions;
using Marten.Schema.Identity;

namespace MartenApi.EventStore.Impl;

public sealed class TransactionSession : ITransactionSession
{
    private readonly Func<IDocumentSession> _sessionFactory;

    private IDocumentSession? _documentSession;

    public TransactionSession(Func<IDocumentSession> sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    public IQuerySession QuerySession => CreateOrGetDocumentSession();

    public string? CausationId
    {
        get => CreateOrGetDocumentSession().CausationId;  
        set => CreateOrGetDocumentSession().CausationId = value;
    }

    public IEventStore Events => CreateOrGetDocumentSession().Events;

    private IDocumentSession CreateOrGetDocumentSession()
    {
        return _documentSession ??= _sessionFactory();
    }

    #region TransactionState

    public async Task<bool> MarkStreamForUpdateIfExists(string streamKey, CancellationToken cancellationToken = default)
    {
        try
        {
            await Events.AppendOptimistic(streamKey, cancellationToken);
            return true;
        }
        catch (NonExistentStreamException)
        {
            return false;
        }
    }

    public PendingChangesSummary PendingChanges()
    {
        return PendingChangesSummary.FromUnitOfWork(_documentSession?.PendingChanges);
    }

    public async ValueTask Rollback()
    {
        await DisposeAsyncCore();
    }

    public string CorrelateEvents(string? existingCorrelationId = null)
    {
        return CreateOrGetDocumentSession().CorrelationId = existingCorrelationId ??
                                                            CreateOrGetDocumentSession().CorrelationId ??
                                                            CombGuidIdGeneration.NewGuid().ToString();
    }

    public async Task Commit(CancellationToken cancellationToken = default)
    {
        if (PendingChanges().AnyChanges)
        {
            await _documentSession!.SaveChangesAsync(cancellationToken);
        }
    }

    #endregion

    #region Disposal

    public void Dispose()
    {
        Dispose(true);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _documentSession?.Dispose();
            _documentSession = null;
        }
    }

    private async ValueTask DisposeAsyncCore()
    {
        if (_documentSession is not null)
        {
            await _documentSession.DisposeAsync().ConfigureAwait(false);
            _documentSession = null;
        }
    }

    #endregion
}