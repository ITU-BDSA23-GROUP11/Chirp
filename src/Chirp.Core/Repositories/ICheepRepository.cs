namespace Chirp.Core.Repositories;
using Dto;

public interface ICheepRepository
{
    public CheepDto? AddCheep(AddCheepDto cheep);
    public int GetCheepCount();
    public int GetAuthorCheepCount(string authorUsername, Guid? authUser = null);
    public List<CheepDto> GetCheepsForPage(int pageNumber);
    public List<CheepDto> GetAuthorCheepsForPage(string authorUsername, int pageNumber);
    public List<CheepDto> GetAuthorCheepsForPageAsOwner(Guid authorId, int pageNumber);

    public List<CheepDto> GetCheepsFromIds(HashSet<Guid> cheepId);
    public bool DeleteCheep(Guid cheepId, Guid authorId);
   
}