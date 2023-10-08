using Chirp.DBService.Models;

namespace Chirp.DBService.Repositories;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _chirpDbContext = new ();
    public CheepRepository()
    {
        DbInitializer.SeedDatabase(_chirpDbContext);
    }
    
    public Cheep AddCheep(Cheep cheep)
    {
        _chirpDbContext.Cheeps.Add(cheep);
        _chirpDbContext.SaveChanges();
        return cheep;
    }

    public List<Cheep> GetCheeps()
    {
        return _chirpDbContext.Cheeps.ToList();
    }

    public List<Cheep> GetCheepsFromAuthorName(string authorName)
    {
        return _chirpDbContext.Cheeps.Where(c => c.Author.Name == authorName).ToList();
    }
}