namespace Chirp.Core.Repositories;
using Dto;

public interface ICheepRepository
{
    public CheepDto AddCheep(AddCheepDto cheep);
    public int GetCheepCount();
    public int GetAuthorCheepCount(string authorName);
    public List<CheepDto> GetCheepsForPage(int pageNumber);
    public List<CheepDto> GetAuthorCheepsForPage(string authorName, int pageNumber);
    public bool DeleteCheep(String cheepId, String author);
    public List<string> GetFollowsForAuthor(string authorEmail);
    public void AddFollow(string authorEmail, string followEmail);
}