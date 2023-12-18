namespace Chirp.Core.Dto;

public class AddCommentDto
{
    public required Guid AuthorId { get; init; }
    public required Guid CheepId { get; init; }
    public required string Text { get; init; }
}

public class CommentDto
{
    public required Guid CommentId { get; init; }
    public required Guid AuthorId { get; init; }
    
    public required Guid CheepAuthorId { get; init; }
    public required Guid CheepId { get; init; }
    public required string? AuthorName { get; init; }
    public required string AuthorUsername { get; init; }
    public required string AuthorAvatarUrl { get; init; }
    public required string Text { get; init; }
    public required DateTime Timestamp { get; init; }
}