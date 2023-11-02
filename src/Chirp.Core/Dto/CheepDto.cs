namespace Chirp.Core.Dto;

public class AddCheepDto
{
    public required string AuthorName { get; init; }
    public required string AuthorEmail { get; init; }
    public required string Text { get; init; }
}

public class CheepDto
{
    public required Guid CheepId { get; init; }
    public required string AuthorName { get; init; }
    public required string Text { get; init; }
    public required DateTime Timestamp { get; init; }
}