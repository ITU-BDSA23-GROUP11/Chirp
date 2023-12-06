namespace Chirp.Core.Dto;

public class LikeDto
{
    public required Guid CheepId { get; init; }
    public required Guid LikedByAuthorId { get; init; }
}