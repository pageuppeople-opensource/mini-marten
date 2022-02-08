using Marten;
using Marten.Events;
using Marten.Exceptions;
using Marten.Linq;

namespace MartenApi.EventStore.Impl;

public sealed class EventTransactionSession : IEventTransactionSession
{
    private readonly Func<IDocumentSession> _sessionFactory;

    private IDocumentSession? _documentSession;

    public IQuerySession QuerySession => CreateOrGetDocumentSession();

    public EventTransactionSession(Func<IDocumentSession> sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }


    private IDocumentSession CreateOrGetDocumentSession()
    {
        return _documentSession ??= _sessionFactory();
    }

    #region IEventStore

    public StreamAction Append(Guid stream, IEnumerable<object> events)
    {
        return CreateOrGetDocumentSession().Events.Append(stream, events);
    }

    public StreamAction Append(Guid stream, params object[] events)
    {
        return CreateOrGetDocumentSession().Events.Append(stream, events);
    }

    public StreamAction Append(string stream, IEnumerable<object> events)
    {
        return CreateOrGetDocumentSession().Events.Append(stream, events);
    }

    public StreamAction Append(string stream, params object[] events)
    {
        return CreateOrGetDocumentSession().Events.Append(stream, events);
    }

    public StreamAction Append(Guid stream, long expectedVersion, params object[] events)
    {
        return CreateOrGetDocumentSession().Events.Append(stream, expectedVersion, events);
    }

    public StreamAction Append(string stream, long expectedVersion, IEnumerable<object> events)
    {
        return CreateOrGetDocumentSession().Events.Append(stream, expectedVersion, events);
    }

    public StreamAction Append(string stream, long expectedVersion, params object[] events)
    {
        return CreateOrGetDocumentSession().Events.Append(stream, expectedVersion, events);
    }

    public StreamAction StartStream<TAggregate>(Guid id, params object[] events) where TAggregate : class
    {
        return CreateOrGetDocumentSession().Events.StartStream<TAggregate>(id, events);
    }

    public StreamAction StartStream(Type aggregateType, Guid id, IEnumerable<object> events)
    {
        return CreateOrGetDocumentSession().Events.StartStream(aggregateType, id, events);
    }

    public StreamAction StartStream(Type aggregateType, Guid id, params object[] events)
    {
        return CreateOrGetDocumentSession().Events.StartStream(aggregateType, id, events);
    }

    public StreamAction StartStream<TAggregate>(string streamKey, IEnumerable<object> events) where TAggregate : class
    {
        return CreateOrGetDocumentSession().Events.StartStream<TAggregate>(streamKey, events);
    }

    public StreamAction StartStream<TAggregate>(string streamKey, params object[] events) where TAggregate : class
    {
        return CreateOrGetDocumentSession().Events.StartStream<TAggregate>(streamKey, events);
    }

    public StreamAction StartStream(Type aggregateType, string streamKey, IEnumerable<object> events)
    {
        return CreateOrGetDocumentSession().Events.StartStream(aggregateType, streamKey, events);
    }

    public StreamAction StartStream(Type aggregateType, string streamKey, params object[] events)
    {
        return CreateOrGetDocumentSession().Events.StartStream(aggregateType, streamKey, events);
    }

    public StreamAction StartStream(Guid id, IEnumerable<object> events)
    {
        return CreateOrGetDocumentSession().Events.StartStream(id, events);
    }

    public StreamAction StartStream(Guid id, params object[] events)
    {
        return CreateOrGetDocumentSession().Events.StartStream(id, events);
    }

    public StreamAction StartStream(string streamKey, IEnumerable<object> events)
    {
        return CreateOrGetDocumentSession().Events.StartStream(streamKey, events);
    }

    public StreamAction StartStream(string streamKey, params object[] events)
    {
        return CreateOrGetDocumentSession().Events.StartStream(streamKey, events);
    }

    public StreamAction StartStream<TAggregate>(IEnumerable<object> events) where TAggregate : class
    {
        return CreateOrGetDocumentSession().Events.StartStream<TAggregate>(events);
    }

    public StreamAction StartStream<TAggregate>(params object[] events) where TAggregate : class
    {
        return CreateOrGetDocumentSession().Events.StartStream<TAggregate>(events);
    }

    public StreamAction StartStream(Type aggregateType, IEnumerable<object> events)
    {
        return CreateOrGetDocumentSession().Events.StartStream(aggregateType, events);
    }

    public StreamAction StartStream(Type aggregateType, params object[] events)
    {
        return CreateOrGetDocumentSession().Events.StartStream(aggregateType, events);
    }

    public StreamAction StartStream(IEnumerable<object> events)
    {
        return CreateOrGetDocumentSession().Events.StartStream(events);
    }

    public StreamAction StartStream(params object[] events)
    {
        return CreateOrGetDocumentSession().Events.StartStream(events);
    }

    public IReadOnlyList<IEvent> FetchStream(Guid streamId, long version = 0, DateTimeOffset? timestamp = null,
        long fromVersion = 0)
    {
        return CreateOrGetDocumentSession().Events.FetchStream(streamId, version, timestamp, fromVersion);
    }

    public Task<IReadOnlyList<IEvent>> FetchStreamAsync(Guid streamId, long version = 0,
        DateTimeOffset? timestamp = null, long fromVersion = 0,
        CancellationToken token = default)
    {
        return CreateOrGetDocumentSession().Events.FetchStreamAsync(streamId, version, timestamp, fromVersion, token);
    }

    public IReadOnlyList<IEvent> FetchStream(string streamKey, long version = 0, DateTimeOffset? timestamp = null,
        long fromVersion = 0)
    {
        return CreateOrGetDocumentSession().Events.FetchStream(streamKey, version, timestamp, fromVersion);
    }

    public Task<IReadOnlyList<IEvent>> FetchStreamAsync(string streamKey, long version = 0,
        DateTimeOffset? timestamp = null, long fromVersion = 0,
        CancellationToken token = default)
    {
        return CreateOrGetDocumentSession().Events.FetchStreamAsync(streamKey, version, timestamp, fromVersion, token);
    }

    public T? AggregateStream<T>(Guid streamId, long version = 0, DateTimeOffset? timestamp = null, T? state = default,
        long fromVersion = 0) where T : class
    {
        return CreateOrGetDocumentSession().Events.AggregateStream(streamId, version, timestamp, state, fromVersion);
    }

    public Task<T?> AggregateStreamAsync<T>(Guid streamId, long version = 0, DateTimeOffset? timestamp = null,
        T? state = default,
        long fromVersion = 0, CancellationToken token = default) where T : class
    {
        return CreateOrGetDocumentSession().Events
            .AggregateStreamAsync(streamId, version, timestamp, state, fromVersion, token);
    }

    public T? AggregateStream<T>(string streamKey, long version = 0, DateTimeOffset? timestamp = null,
        T? state = default,
        long fromVersion = 0) where T : class
    {
        return CreateOrGetDocumentSession().Events.AggregateStream(streamKey, version, timestamp, state, fromVersion);
    }

    public Task<T?> AggregateStreamAsync<T>(string streamKey, long version = 0, DateTimeOffset? timestamp = null,
        T? state = default, long fromVersion = 0, CancellationToken token = default) where T : class
    {
        return CreateOrGetDocumentSession().Events
            .AggregateStreamAsync(streamKey, version, timestamp, state, fromVersion, token);
    }

    public IMartenQueryable<T> QueryRawEventDataOnly<T>()
    {
        return CreateOrGetDocumentSession().Events.QueryRawEventDataOnly<T>();
    }

    public IMartenQueryable<IEvent> QueryAllRawEvents()
    {
        return CreateOrGetDocumentSession().Events.QueryAllRawEvents();
    }

    public IEvent<T> Load<T>(Guid id) where T : class
    {
        return CreateOrGetDocumentSession().Events.Load<T>(id);
    }

    public Task<IEvent<T>> LoadAsync<T>(Guid id, CancellationToken token = default) where T : class
    {
        return CreateOrGetDocumentSession().Events.LoadAsync<T>(id, token);
    }

    public IEvent Load(Guid id)
    {
        return CreateOrGetDocumentSession().Events.Load(id);
    }

    public Task<IEvent> LoadAsync(Guid id, CancellationToken token = default)
    {
        return CreateOrGetDocumentSession().Events.LoadAsync(id, token);
    }

    public StreamState FetchStreamState(Guid streamId)
    {
        return CreateOrGetDocumentSession().Events.FetchStreamState(streamId);
    }

    public Task<StreamState> FetchStreamStateAsync(Guid streamId, CancellationToken token = default)
    {
        return CreateOrGetDocumentSession().Events.FetchStreamStateAsync(streamId, token);
    }

    public StreamState FetchStreamState(string streamKey)
    {
        return CreateOrGetDocumentSession().Events.FetchStreamState(streamKey);
    }

    public Task<StreamState> FetchStreamStateAsync(string streamKey, CancellationToken token = default)
    {
        return CreateOrGetDocumentSession().Events.FetchStreamStateAsync(streamKey, token);
    }

    public StreamAction Append(Guid stream, long expectedVersion, IEnumerable<object> events)
    {
        return CreateOrGetDocumentSession().Events.Append(stream, expectedVersion, events);
    }

    public StreamAction StartStream<TAggregate>(Guid id, IEnumerable<object> events) where TAggregate : class
    {
        return CreateOrGetDocumentSession().Events.StartStream<TAggregate>(id, events);
    }

    public Task AppendOptimistic(string streamKey, CancellationToken token, params object[] events)
    {
        return CreateOrGetDocumentSession().Events.AppendOptimistic(streamKey, token, events);
    }

    public Task AppendOptimistic(string streamKey, params object[] events)
    {
        return CreateOrGetDocumentSession().Events.AppendOptimistic(streamKey, events);
    }

    public Task AppendOptimistic(Guid streamId, CancellationToken token, params object[] events)
    {
        return CreateOrGetDocumentSession().Events.AppendOptimistic(streamId, token, events);
    }

    public Task AppendOptimistic(Guid streamId, params object[] events)
    {
        return CreateOrGetDocumentSession().Events.AppendOptimistic(streamId, events);
    }

    public Task AppendExclusive(string streamKey, CancellationToken token, params object[] events)
    {
        return CreateOrGetDocumentSession().Events.AppendExclusive(streamKey, token, events);
    }

    public Task AppendExclusive(string streamKey, params object[] events)
    {
        return CreateOrGetDocumentSession().Events.AppendExclusive(streamKey, events);
    }

    public Task AppendExclusive(Guid streamId, CancellationToken token, params object[] events)
    {
        return CreateOrGetDocumentSession().Events.AppendExclusive(streamId, token, events);
    }

    public Task AppendExclusive(Guid streamId, params object[] events)
    {
        return CreateOrGetDocumentSession().Events.AppendExclusive(streamId, events);
    }

    public void ArchiveStream(Guid streamId)
    {
        CreateOrGetDocumentSession().Events.ArchiveStream(streamId);
    }

    public void ArchiveStream(string streamKey)
    {
        CreateOrGetDocumentSession().Events.ArchiveStream(streamKey);
    }

    #endregion

    #region TransactionState
    
    public async Task<bool> MarkStreamForUpdateIfExists(Guid streamId, CancellationToken cancellationToken = default)
    {
        try
        {
            await AppendOptimistic(streamId, cancellationToken);
            return true;
        }
        catch (NonExistentStreamException)
        {
            return false;
        }
    }

    public async Task<bool> MarkStreamForUpdateIfExists(string streamKey, CancellationToken cancellationToken = default)
    {
        try
        {
            await AppendOptimistic(streamKey, cancellationToken);
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