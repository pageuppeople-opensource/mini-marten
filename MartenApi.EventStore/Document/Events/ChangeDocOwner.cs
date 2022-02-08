namespace MartenApi.EventStore.Document.Events;

public record ChangeDocOwner
{
    public string NewOwner { get; set; }
}