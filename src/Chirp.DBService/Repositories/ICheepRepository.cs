using Chirp.DBService.Models;

namespace Chirp.DBService.Repositories;

public interface ICheepRepository
{
    public Cheep AddCheep(Cheep cheep);
    public List<Cheep> GetCheeps();
    public List<Cheep> GetCheepsFromAuthorName(string authorName);
}