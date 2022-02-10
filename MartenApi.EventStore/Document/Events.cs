namespace MartenApi.EventStore.Document;

public record DocumentCreated(long DocumentId, string Owner, string Title, string Content);
public record DocumentOwnerChanged(long DocumentId, string OldOwner, string NewOwner);
public record DocumentContentUpdated(long DocumentId, string Content);
public record DocumentTitleUpdated(long DocumentId, string Title);
