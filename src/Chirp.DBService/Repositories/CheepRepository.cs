using Chirp.DBService.Contexts;
using Chirp.DBService.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.DBService.Repositories;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDbContext _chirpDbContext;

    public CheepRepository(ChirpDbContext chirpDbContext)
    {
        _chirpDbContext = chirpDbContext;
    }
    
    public Cheep AddCheep(Cheep cheep)
    {
        _chirpDbContext.Cheeps.Add(cheep);
        _chirpDbContext.SaveChanges();
        return cheep;
    }
    
    public void DeleteCheep(Cheep cheep)
    {
        _chirpDbContext.Cheeps.Remove(cheep);
        _chirpDbContext.SaveChanges();
    }
    
    public int GetCheepCount()
    {
        return _chirpDbContext.Cheeps.Count();
    }

    public List<Cheep> GetCheepsWithAuthors()
    {
        return _chirpDbContext.Cheeps
            .Include(c => c.Author)
            .ToList();
    }
    
    public List<Cheep> GetCheepsForPage(int pageNumber)
    {
        return GetCheepsWithAuthors()
            .Skip((pageNumber - 1) * 32)
            .Take(32)//Refactor
            .ToList();
    }

    public List<Cheep> GetCheepsFromAuthorNameWithAuthors(string authorName)
    {
        return _chirpDbContext.Cheeps.Where(c => c.Author.Name == authorName)
            .Include(c => c.Author)
            .ToList();
    }

    public List<Cheep> GetCheepsFromAuthorNameForPage(string authorName, int pageNumber)
    {
        return GetCheepsFromAuthorNameWithAuthors(authorName)
            .Skip((pageNumber - 1) * 32)
            .Take(32)//Refactor
            .ToList();
    }
}