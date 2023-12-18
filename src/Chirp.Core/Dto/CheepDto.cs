namespace Chirp.Core.Dto;

public class AddCheepDto
{
    public required Guid AuthorId { get; init; }
    public required string Text { get; init; }
}

public class CheepDto
{
    public required Guid CheepId { get; init; }
    public required Guid AuthorId { get; init; }
    public required string? AuthorName { get; init; }
    public required string AuthorUsername { get; init; }
    public required string AuthorAvatarUrl { get; init; }
    public required string Text { get; init; }
    public required DateTime Timestamp { get; init; }

    public required List<CommentDto> CommentDtos { get; init; }
}