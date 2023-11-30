using Chirp.Core.Dto;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Contexts;

namespace Chirp.Infrastructure.Repositories;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDbContext _chirpDbContext;
    private readonly IAuthorRepository _authorRepository;

    public CheepRepository(ChirpDbContext chirpDbContext, IAuthorRepository authorRepository)
    {
        _chirpDbContext = chirpDbContext;
        _authorRepository = authorRepository;
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
            AuthorEmail = newCheep.Author.Email,
            Text = newCheep.Text,
            Timestamp = newCheep.Timestamp
        };
    }

    public string GetCheepById(string id)
    {
        foreach (var cheep in _chirpDbContext.Cheeps)
        {
            if (cheep.CheepId.ToString().Equals(id))
            {
                return cheep.CheepId.ToString();
            }
        }

        return "";
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
        return FetchWithErrorHandling(() =>
        {
            return _chirpDbContext
                .Cheeps
                .Include(c => c.Author)
                .OrderByDescending(c => c.Timestamp)
                .Skip(int.Max(pageNumber - 1, 0) * 32)
                .Take(32)
                .Select<Cheep, CheepDto>(c =>
                    new CheepDto {
                        CheepId = c.CheepId,
                        AuthorName = c.Author.Name,
                        AuthorEmail = c.Author.Email,
                        Text = c.Text,
                        Timestamp = c.Timestamp
                    }
                )
                .ToList();
        });
    }

    public List<CheepDto> GetAuthorCheepsForPage(string authorName, int pageNumber)
    {
        return FetchWithErrorHandling(() =>
        { 
            return _chirpDbContext
                .Cheeps
                .Where(c => c.Author.Name == authorName)
                .Include(c => c.Author)
                .OrderByDescending(c => c.Timestamp)
                .Skip(int.Max(pageNumber - 1, 0) * 32)
                .Take(32)
                .Select<Cheep, CheepDto>(c =>
                    new CheepDto {
                        CheepId = c.CheepId,
                        AuthorName = c.Author.Name,
                        AuthorEmail = c.Author.Email,
                        Text = c.Text,
                        Timestamp = c.Timestamp
                    }
                )
                .ToList();
        });
    }

    public List<CheepDto> GetAuthorCheepsForPageAsOwner(string authorName, int pageNumber)
    {
        return FetchWithErrorHandling(() =>
        {
            var email = _chirpDbContext.Authors.Single(a => a.Name == authorName).Email;
            List<string> authorFollows = _authorRepository.GetFollowsForAuthor(email);
            return _chirpDbContext
                .Cheeps
                .Where(c => authorFollows.Contains(c.Author.Email) || c.Author.Name == authorName)
                .Include(c => c.Author)
                .OrderByDescending(c => authorFollows.Contains(c.Author.Name))
                .ThenBy(c => c.Timestamp)
                .Skip(int.Max(pageNumber - 1, 0) * 32)
                .Take(32)
                .Select<Cheep, CheepDto>(c =>
                    new CheepDto {
                        CheepId = c.CheepId,
                        AuthorName = c.Author.Name,
                        AuthorEmail = c.Author.Email,
                        Text = c.Text,
                        Timestamp = c.Timestamp
                    }
                )
                .ToList();
        });
    }

    private List<CheepDto> FetchWithErrorHandling(Func<List<CheepDto>> fetchFunction)
    {
        try
        {
            return fetchFunction();
        }
        catch
        {
            return new List<CheepDto>();
        }
    }
    
    public bool DeleteCheep(Guid cheepId, Guid authorId)
    {
        Cheep? cheepToDelete = _chirpDbContext.Cheeps
            .Include(c => c.Author)
            .SingleOrDefault(c => c.CheepId == cheepId);

        if (cheepToDelete == null) return false;
        if (cheepToDelete.Author.AuthorId.Equals(authorId)) return false;
        
        _chirpDbContext.Cheeps.Remove(cheepToDelete);
        _chirpDbContext.SaveChanges();
        
        return true; 
    }
}