﻿using Marten;
using Marten.Schema.Identity;
using MartenApi.EventStore.Document.Projections;
using MartenApi.EventStore.Impl;

namespace MartenApi.EventStore.Document;

public class DocumentService : IDocumentService
{
    private readonly IEntityIdProvider _entityIdProvider;

    public DocumentService(IEntityIdProvider entityIdProvider)
    {
        _entityIdProvider = entityIdProvider;
    }

    public async Task<Projections.Document?> TryGetDocumentById(IQuerySession querySession, long documentId,
        CancellationToken token = default)
    {
        var streamKey = await TryGetDocumentStreamKeyById(querySession, documentId, token);
        if (streamKey is null)
        {
            return null;
        }

        return await TryGetDocumentByStreamKey(querySession, streamKey, token);
    }

    public IAsyncEnumerable<DocumentSearch> SearchDocuments(IQuerySession querySession, string? searchQuery,
        int page = 0, int pageSize = 10,
        CancellationToken token = default)
    {
        if (page <= 1)
        {
            page = 1;
        }

        if (pageSize <= 0)
        {
            return AsyncEnumerable.Empty<DocumentSearch>();
        }

        var query = querySession.Query<DocumentSearch>() as IQueryable<DocumentSearch>;

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            query = query.Where(x => x.PlainTextSearch(searchQuery));
        }

        return query.Skip((page - 1) * pageSize).Take(pageSize)
            .ToAsyncEnumerable(token);
    }

    public async Task<Projections.Document?> TryGetDocumentByStreamKey(IQuerySession querySession, string streamKey,
        CancellationToken token = default)
    {
        return await querySession.Events.AggregateStreamAsync<Projections.Document>(streamKey, 5, token: token);
    }

    public async Task<string?> TryGetDocumentStreamKeyById(IQuerySession querySession, long documentId,
        CancellationToken token = default)
    {
        var streamKey = await querySession.Query<DocumentKeymap>().Where(x => x.DocumentId == documentId)
            .Select(x => x.StreamKey).SingleOrDefaultAsync(token);

        return streamKey;
    }

    public IAsyncEnumerable<DocumentOwner> GetDocumentOwners(IQuerySession querySession,
        CancellationToken token = default)
    {
        return querySession.Query<DocumentOwner>().OrderBy(d => d.Owner).ToAsyncEnumerable(token);
    }

    public async Task<Projections.Document> CreateDocument(ITransactionSession session, string owner, string title,
        string content,
        CancellationToken token = default)
    {
        var newDocumentId = await _entityIdProvider.GetNextId<Projections.Document>(session.QuerySession, token);
        var streamKey = CombGuidIdGeneration.NewGuid().ToString();
        var createEvent = new DocumentCreated(newDocumentId, owner, title, content);

        session.Events.StartStream<Projections.Document>(streamKey, createEvent);
        return Projections.Document.Create(createEvent, streamKey);
    }

    public async Task<Projections.Document> UpdateDocument(ITransactionSession session, Projections.Document document,
        string title, string content, CancellationToken token = default)
    {
        session.CorrelateEvents();

        var newDoc = document;

        if (document.Title != title)
        {
            var updateEvent = new DocumentTitleUpdated(document.DocumentId, title);
            await session.Events.AppendOptimistic(document.StreamKey, token, updateEvent);
            newDoc = newDoc.Apply(updateEvent, newDoc);
        }

        if (document.Content != content)
        {
            var updateEvent = new DocumentContentUpdated(document.DocumentId, content);
            await session.Events.AppendOptimistic(document.StreamKey, token, updateEvent);
            newDoc = newDoc.Apply(updateEvent, newDoc);
        }

        return newDoc;
    }

    public async Task<Projections.Document> UpdateDocumentOwner(ITransactionSession session,
        Projections.Document document,
        string newOwner,
        CancellationToken token = default)
    {
        var changeEvent = new DocumentOwnerChanged(document.DocumentId, document.Owner, newOwner);
        await session.Events.AppendOptimistic(document.StreamKey, token, changeEvent);

        return document.Apply(changeEvent, document);
    }
}