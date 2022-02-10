namespace MartenApi.EventStore.Document;

public record CreateDoc(long DocumentId, string Owner, string Content);
public record ChangeDocOwner(long DocumentId, string OldOwner, string NewOwner);
public record UpdateDoc(long DocumentId, string Content);
