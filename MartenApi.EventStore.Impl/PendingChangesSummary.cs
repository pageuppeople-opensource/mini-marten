using Marten.Services;

namespace MartenApi.EventStore.Impl;

public record PendingChangesSummary
{
    public int DocumentInserts { get; init; }
    public int DocumentUpdates { get; init; }
    public int DocumentDeletions { get; init; }
    public int StreamWrites { get; init; }
    public int TotalOperations { get; init; }
    public IUnitOfWork? UnitOfWork { get; init; }

    public bool AnyChanges => DocumentInserts > 0 || DocumentUpdates > 0 || DocumentDeletions > 0 || StreamWrites > 0 ||
                              TotalOperations > 0;

    public static PendingChangesSummary FromUnitOfWork(IUnitOfWork? unitOfWork)
    {
        if (unitOfWork == null)
        {
            return new PendingChangesSummary();
        }

        return new PendingChangesSummary
        {
            UnitOfWork = unitOfWork,
            DocumentDeletions = unitOfWork.Deletions().Count(),
            DocumentUpdates = unitOfWork.Updates().Count(),
            StreamWrites = unitOfWork.Streams().Count,
            DocumentInserts = unitOfWork.Inserts().Count(),
            TotalOperations = unitOfWork.Operations().Count()
        };
    }
}