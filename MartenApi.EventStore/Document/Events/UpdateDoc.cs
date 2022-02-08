namespace MartenApi.EventStore.Document.Events;

public record UpdateDoc
{
    public string Content { get; set; }
}