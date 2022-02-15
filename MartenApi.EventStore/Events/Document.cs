namespace MartenApi.EventStore.Events;

public record DocumentCreated(DocumentId DocumentId, string Owner, string Title, string Content);
public record DocumentOwnerChanged(DocumentId DocumentId, string OldOwner, string NewOwner);
public record DocumentContentUpdated(DocumentId DocumentId, string Content);
public record DocumentTitleUpdated(DocumentId DocumentId, string Title);
