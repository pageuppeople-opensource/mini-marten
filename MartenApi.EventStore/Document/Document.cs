namespace MartenApi.EventStore.Document;

/// <summary>
/// Note: Everything is immutable
/// </summary>

public record Document
{
    public Guid Id { get; init; }
    public string? Owner { get; init; }
    public string? Content { get; init; }
}
