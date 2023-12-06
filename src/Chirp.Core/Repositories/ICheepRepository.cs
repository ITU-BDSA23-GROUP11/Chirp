namespace Chirp.Core.Repositories;
using Dto;

public interface ICheepRepository
{
    public CheepDto? AddCheep(AddCheepDto cheep);
    public int GetCheepCount();
    public int GetAuthorCheepCount(string authorLogin, bool withFollows = false);
    public List<CheepDto> GetCheepsForPage(int pageNumber);
    public List<CheepDto> GetAuthorCheepsForPage(string authorLogin, int pageNumber);
    public List<CheepDto> GetAuthorCheepsForPageAsOwner(Guid authorId, int pageNumber);
    public bool DeleteCheep(Guid cheepId, Guid authorId);
}