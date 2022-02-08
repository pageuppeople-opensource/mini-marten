namespace MartenApi.EventStore.Impl;

public class UncommittedChangesException : Exception
{
    public UncommittedChangesException(PendingChangesSummary summary) : base(
        $"Transaction scope ended with pending changes.\nPlease review code and ensure that all changes are committed or rolled back before exiting a transaction scope.\nDetails: {summary}")
    {
        PendingChanges = summary;
    }

    public PendingChangesSummary PendingChanges { get; }
}