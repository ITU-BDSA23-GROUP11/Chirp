using Chirp.Core.Dto;

namespace Chirp.Core.Repositories;

public interface IAuthorRepository
{
    public void AddAuthor(AuthorDto authorDto);
    public List<string> GetFollowsForAuthor(string authorEmail);
    public string GetAuthorNameByEmail(string authorEmail);
    
    public void AddFollow(string authorEmail, string followEmail);

    public void RemoveFollow(string authorEmail, string unfollowEmail);
}