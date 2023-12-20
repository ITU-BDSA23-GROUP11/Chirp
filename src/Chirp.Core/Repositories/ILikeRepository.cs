using Chirp.Core.Dto;

namespace Chirp.Core.Repositories;

public interface ILikeRepository
{
    public Task LikeCheep(Guid authorId, Guid cheepId);
    public Task UnlikeCheep(Guid authorId, Guid cheepId);
    public Task<int> LikeCount(Guid cheepId);
    public Task<bool> IsLiked(Guid authorId, Guid cheepId);
    public Task<LikeDto> GetLike(Guid authorId, Guid cheepId);
    public Task<List<LikeDto>> GetLikesByAuthorId(Guid authorId);
    public Task<List<LikeDto>> GetLikesByCheepId(Guid cheepId);
}