using Chirp.Core.Dto;

namespace Chirp.Core.Repositories;

public interface IAuthorRepository
{
    public Task AddAuthor(AuthorDto authorDto);
    public Task<List<string>> GetFollowsForAuthor(Guid authorId);
    public Task AddFollow(Guid authorId, Guid followId);
    public Task RemoveFollow(Guid authorId, Guid unfollowAuthorId);
    public Task<bool> DeleteAuthor(Guid authorId);
}