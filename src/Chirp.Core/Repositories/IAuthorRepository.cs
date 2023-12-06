using Chirp.Core.Dto;

namespace Chirp.Core.Repositories;

public interface IAuthorRepository
{
    public void AddAuthor(AuthorDto authorDto);
    
    /// <returns>The logins of the users following the author with id authorId</returns>
    public List<string> GetFollowsForAuthor(Guid authorId);
    /// <returns>The logins of the users following the author with id authorId</returns>
    public List<string> GetFollowsForAuthor(string authorLogin);

    public AuthorDto? GetAuthorFromLogin(string authorLogin);
    
    public void AddFollow(Guid authorId, Guid followId);

    public void RemoveFollow(Guid authorId, Guid unfollowAuthorId);
}