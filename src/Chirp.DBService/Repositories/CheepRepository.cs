using Chirp.DBService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Chirp.DBService.Repositories;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _chirpDbContext;
    public CheepRepository(bool isTest = false)
    {
        _chirpDbContext = new ChirpDBContext(isTest);
        var dbCreator = _chirpDbContext.GetService<IRelationalDatabaseCreator>();
        dbCreator.EnsureCreated();
        if (!isTest) DbInitializer.SeedDatabase(_chirpDbContext);
    }
    
    public Cheep AddCheep(Cheep cheep)
    {
        _chirpDbContext.Cheeps.Add(cheep);
        _chirpDbContext.SaveChanges();
        return cheep;
    }
    
    public int GetCheepCount()
    {
        return _chirpDbContext.Cheeps.Count();
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

    public List<Cheep> GetCheepsFromAuthorNameWithoutAuthors(string authorName)
    {
        return _chirpDbContext.Cheeps.Where(c => c.Author.Name == authorName).ToList();
    }

    public void DeleteDatabase()
    {
        _chirpDbContext.Database.EnsureDeleted();
    }

    public List<Cheep> GetCheepsFromAuthorNameForPage(string authorName, int pageNumber)
    {
        return GetCheepsFromAuthorNameWithAuthors(authorName)
            .Skip((pageNumber - 1) * 32)
            .Take(32)//Refactor
            .ToList();
    }
}