using Chirp.DBService.Models;
using Microsoft.EntityFrameworkCore;

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
    
    public List<Cheep> GetCheepsWithoutAuthors()
    {
        return _chirpDbContext.Cheeps.ToList();
    }

    public List<Cheep> GetCheepsWithAuthors()
    {
        return _chirpDbContext.Cheeps
            .Include(c => c.Author)
            .ToList();
    }
    
    public List<Cheep> GetCheepsForPage(int pageNumber)
    {
       return _chirpDbContext.Cheeps.Skip(pageNumber * 32).Take(32).ToList();
    }

    public int GetCheepCount()
    {
        return _chirpDbContext.Cheeps.Count();
    }

    public List<Cheep> GetCheepsFromAuthorNameWithAuthors(string authorName)
    {
        return _chirpDbContext.Cheeps.Where(c => c.Author.Name == authorName)
            .Include(c => c.Author)
            .ToList();
    }

    public List<Cheep> GetCheepsFromAuthorNameWithoutAuthors(string authorName)
    {
        return _chirpDbContext.Cheeps.Where(c => c.Author.Name == authorName).ToList();
    }
}