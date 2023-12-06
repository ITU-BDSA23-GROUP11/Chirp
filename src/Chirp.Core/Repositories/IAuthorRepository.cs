using Chirp.Core.Dto;

namespace Chirp.Core.Repositories;

public interface IAuthorRepository
{
    public void AddAuthor(AuthorDto authorDto);
    public List<string> GetFollowsForAuthor(string authorLogin);
    
    public void AddFollow(string authorLogin, string followLogin);

    public void RemoveFollow(string authorLogin, string unfollowLogin);
}