using Chirp.Core.Dto;

namespace Chirp.Core.Repositories;

public interface IAuthorRepository
{
    public void AddAuthor(AuthorDto authorDto);
    
    /// <returns>The usernames of the users following the author with id authorId</returns>
    public List<string> GetFollowsForAuthor(Guid authorId);
    /// <returns>The usernames of the users following the author with id authorId</returns>
    public List<string> GetFollowsForAuthor(string authorUsername);

    public AuthorDto? GetAuthorFromUsername(string authorUsername);
    
    public void AddFollow(Guid authorId, Guid followId);

    public void RemoveFollow(Guid authorId, Guid unfollowAuthorId);

    public bool DeleteAuthor(Guid authorId);
}