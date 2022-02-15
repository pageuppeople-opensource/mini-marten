using MartenApi.EventStore.Impl;
using MartenApi.EventStore.Projections.Document;

namespace MartenApi.EventStore.Command;

public interface IDocumentCommandService
{
    Task<DocumentDetail> CreateDocument(ITransactionSession session, string owner, string title,
        string content,
        CancellationToken token = default);

    Task<DocumentDetail> UpdateDocument(ITransactionSession session,
        DocumentDetail documentDetail,
        string title, string content,
        CancellationToken token = default);

    Task<DocumentDetail> UpdateDocumentOwner(ITransactionSession session,
        DocumentDetail documentDetail,
        string newOwner,
        CancellationToken token = default);
}