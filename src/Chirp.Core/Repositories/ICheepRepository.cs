namespace Chirp.Core.Repositories;
using Dto;

public interface ICheepRepository
{
    public CheepDto AddCheep(AddCheepDto cheep);
    public int GetCheepCount();
    public int GetAuthorCheepCount(string authorName);
    public List<CheepDto> GetCheepsForPage(int pageNumber);
    public List<CheepDto> GetAuthorCheepsForPage(string authorName, int pageNumber);

    public List<CheepDto> GetAllCheepsFromAuthor(string authorEmail);
    
    public bool DeleteCheep(String cheepId, String author);
    public List<string> GetFollowsForAuthor(string authorEmail);
    public string GetAuthorEmailByName(string authorName);
    
    public void AddFollow(string authorEmail, string followEmail);

    public void RemoveFollow(string authorEmail, string unfollowEmail);
}