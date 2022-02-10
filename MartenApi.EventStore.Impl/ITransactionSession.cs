using Marten;
using Marten.Events;

namespace MartenApi.EventStore.Impl;

public interface ITransactionSession : IDisposable, IAsyncDisposable
{
    /// <summary>
    ///     Optional metadata describing the correlation id for this
    ///     unit of work
    /// </summary>
    string? CausationId { get; set; }

    /// <summary>
    ///     The events store
    /// </summary>
    IEventStore Events { get; }

    /// <summary>
    ///     The query session for read-only query operations.
    /// </summary>
    IQuerySession QuerySession { get; }

    /// <summary>
    ///     Remember a stream's version at this point in time and ensure on commit that the version hasn't changed.
    /// </summary>
    /// <param name="streamKey">The stream to check</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /></param>
    /// <returns>True if the stream exists, else false</returns>
    Task<bool> MarkStreamForUpdateIfExists(string streamKey, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get a summary of all pending changes.
    /// </summary>
    /// <returns>
    ///     The summary of all pending changes.
    /// </returns>
    PendingChangesSummary PendingChanges();

    /// <summary>
    ///     Reset the current session, no pending changes will be saved.
    /// </summary>
    /// <returns>The <see cref="ValueTask" /></returns>
    ValueTask Rollback();

    /// <summary>
    ///     Associate all events until with a generated correlation id until the next commit.
    /// </summary>
    /// <param name="existingCorrelationId">
    ///     An existing correlation id to use, one is generated if not provided.
    /// </param>
    /// <returns>
    ///     The correlationId
    /// </returns>
    string CorrelateEvents(string? existingCorrelationId = null);

    /// <summary>
    /// Write all pending changes to the event store.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /></param>
    /// <returns></returns>
    Task Commit(CancellationToken cancellationToken = default);
}