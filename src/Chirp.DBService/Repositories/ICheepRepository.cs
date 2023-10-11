using Chirp.DBService.Models;

namespace Chirp.DBService.Repositories;

public interface ICheepRepository
{
    public Cheep AddCheep(Cheep cheep);
    public List<Cheep> GetCheepsWithoutAuthors();
    public List<Cheep> GetCheepsWithAuthors();
    public List<Cheep> GetCheepsForPage(int pageNumber);
    public int GetCheepCount();
    public List<Cheep> GetCheepsFromAuthorNameWithAuthors(string authorName);
    public List<Cheep> GetCheepsFromAuthorNameWithoutAuthors(string authorName);

    public List<Cheep> GetCheepsFromAuthorNameForPage(string authorName, int pageNumber);
    public void DeleteDatabase();
}