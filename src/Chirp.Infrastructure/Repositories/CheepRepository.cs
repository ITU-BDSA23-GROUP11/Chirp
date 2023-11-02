using Chirp.Core.Dto;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Contexts;

namespace Chirp.Infrastructure.Repositories;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDbContext _chirpDbContext;

    public CheepRepository(ChirpDbContext chirpDbContext)
    {
        _chirpDbContext = chirpDbContext;
    }
    
    public CheepDto AddCheep(AddCheepDto cheep)
    {
        Author author =
            _chirpDbContext.Authors.FirstOrDefault(a => a.Name == cheep.AuthorName && a.Email == cheep.AuthorEmail) ??
            new Author
            {
                Email = cheep.AuthorEmail,
                Name = cheep.AuthorName
            };

        Cheep newCheep = new Cheep
        {
            Author = author,
            Text = cheep.Text,
        };
        
        _chirpDbContext.Cheeps.Add(newCheep);
        _chirpDbContext.SaveChanges();

        return new CheepDto
        {
            CheepId = newCheep.CheepId,
            AuthorName = newCheep.Author.Name,
            Text = newCheep.Text,
            Timestamp = newCheep.Timestamp
        };
    }
    
    public int GetCheepCount()
    {
        return _chirpDbContext.Cheeps.Count();
    }
    
    public int GetAuthorCheepCount(string authorName)
    {
        return _chirpDbContext.Cheeps.Count(c => c.Author.Name == authorName);
    }
    
    public List<CheepDto> GetCheepsForPage(int pageNumber)
    {
        return _chirpDbContext
            .Cheeps
            .Include(c => c.Author)
            .Skip((pageNumber - 1) * 32)
            .Take(32)
            .Select<Cheep, CheepDto>(c =>
                new CheepDto {
                    CheepId = c.CheepId,
                    AuthorName = c.Author.Name,
                    Text = c.Text,
                    Timestamp = c.Timestamp
                }
            )
            .OrderByDescending(c => c.Timestamp)
            .ToList();
    }

    public List<CheepDto> GetAuthorCheepsForPage(string authorName, int pageNumber)
    {
        return _chirpDbContext
            .Cheeps
            .Where(c => c.Author.Name == authorName)
            .Include(c => c.Author)
            .Skip((pageNumber - 1) * 32)
            .Take(32)
            .Select<Cheep, CheepDto>(c =>
                new CheepDto {
                    CheepId = c.CheepId,
                    AuthorName = c.Author.Name,
                    Text = c.Text,
                    Timestamp = c.Timestamp
                }
            )
            .OrderByDescending(c => c.Timestamp)
            .ToList();
    }

    public void DeleteCheep(Guid cheepId)
    {
        var cheepToDelete = _chirpDbContext.Cheeps.Find(cheepId);
        if (cheepToDelete != null)
        {
            _chirpDbContext.Cheeps.Remove(cheepToDelete);
            _chirpDbContext.SaveChanges();
        }
}
}