using Chirp.Core.DTO;
using Chirp.Infrastructure.Models;
using Chirp.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.Repositories;

namespace Chirp.Infrastructure.Repositories;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _chirpDbContext;

    public CheepRepository(ChirpDBContext chirpDbContext)
    {
        Console.WriteLine("STARTING!!!");
        _chirpDbContext = chirpDbContext;
        //_chirpDbContext.Database.Migrate();
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
    
    
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _chirpDbContext.Dispose();
            }
        }
        disposed = true;
    }

    public void Dispose()
    {
        Console.WriteLine("DISPOSING!!!");
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}