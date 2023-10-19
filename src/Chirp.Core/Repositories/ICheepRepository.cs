namespace Chirp.Core.Repositories;
using Chirp.Core.DTO;

public interface ICheepRepository : IDisposable
{
    public void DeleteCheep(CheepDto cheep);
    public CheepDto AddCheep(CheepDto cheep);
    public List<CheepDto> GetCheepsWithAuthors();
    public List<CheepDto> GetCheepsForPage(int pageNumber);
    public int GetCheepCount();
    public List<CheepDto> GetCheepsFromAuthorNameWithAuthors(string authorName);

    public List<CheepDto> GetCheepsFromAuthorNameForPage(string authorName, int pageNumber);
}