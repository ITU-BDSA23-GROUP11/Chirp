namespace Chirp.Core.Repositories;

public interface ILikeRepository
{
    public void LikeCheep(Guid authorId, Guid cheepId);
    public void UnlikeCheep(Guid authorId, Guid cheepId);
    public int LikeCount(Guid cheepId);
    public bool isLiked(Guid authorId, Guid cheepId);
}