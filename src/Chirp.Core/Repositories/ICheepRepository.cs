namespace Chirp.Core.Repositories;
using Dto;

public interface ICheepRepository
{
    public Task<CheepDto?> AddCheep(AddCheepDto cheep);
    public Task<int> GetCheepCount();
    public Task<int> GetAuthorCheepCount(string authorUsername, Guid? authUser = null);
    public Task<List<CheepDto>> GetCheepsForPage(int pageNumber);
    public Task<List<CheepDto>> GetAuthorCheepsForPage(string authorUsername, int pageNumber);
    public Task<List<CheepDto>> GetAuthorCheepsForPageAsOwner(Guid authorId, int pageNumber);
    public Task<List<CheepDto>> GetCheepsFromIds(HashSet<Guid> cheepId);
    public Task<bool> DeleteCheep(Guid cheepId);
}