namespace Chirp.Core.Dto;

public class LikeDto
{
    public Guid CheepId { get; set; }
    public Guid LikedByAuthorId { get; set; }
}