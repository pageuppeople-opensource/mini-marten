namespace MartenApi.EventStore.Document;

public record CreateDoc(string DocumentId, string Owner, string Content);
public record ChangeDocOwner(string DocumentId, string NewOwner);
public record UpdateDoc (string DocumentId, string Content);