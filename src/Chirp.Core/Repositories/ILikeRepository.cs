using Chirp.Core.Dto;

namespace Chirp.Core.Repositories;

public interface ILikeRepository
{
    public void LikeCheep(Guid authorId, Guid cheepId);
    public void UnlikeCheep(Guid authorId, Guid cheepId);
    public int LikeCount(Guid cheepId);
    public bool IsLiked(Guid authorId, Guid cheepId);
    public LikeDto GetLike(Guid authorId, Guid cheepId);
    public List<LikeDto> GetLikesByAuthorId(Guid authorId);
    public List<LikeDto> GetLikesByCheepId(Guid cheepId);
}