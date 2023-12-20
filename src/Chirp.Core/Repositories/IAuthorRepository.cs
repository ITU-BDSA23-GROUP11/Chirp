using Chirp.Core.Dto;

namespace Chirp.Core.Repositories;

public interface IAuthorRepository
{
    public Task AddAuthor(AuthorDto authorDto);
    
    /// <returns>The usernames of the users following the author with id authorId</returns>
    public Task<List<string>> GetFollowsForAuthor(Guid authorId);
    /// <returns>The usernames of the users following the author with id authorId</returns>
    public Task<List<string>> GetFollowsForAuthor(string authorUsername);
    public Task<AuthorDto?> GetAuthorFromUsername(string authorUsername);
    public Task AddFollow(Guid authorId, Guid followId);
    public Task RemoveFollow(Guid authorId, Guid unfollowAuthorId);

    public Task<bool> DeleteAuthor(Guid authorId);
}