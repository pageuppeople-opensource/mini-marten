namespace MartenApi.EventStore.Document.Events;

public record CreateDoc
{
    public string Owner { get; set; }

    public string Content { get; set; }
}