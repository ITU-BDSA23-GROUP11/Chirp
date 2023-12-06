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
    
    public CheepDto? AddCheep(AddCheepDto cheep)
    {
        Author? author = _chirpDbContext.Authors.FirstOrDefault(a => a.Login == cheep.AuthorLogin);
        
        if (author == null) return null;

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
            AuthorLogin = newCheep.Author.Login,
            Text = newCheep.Text,
            Timestamp = newCheep.Timestamp
        };
    }
    
    public int GetCheepCount()
    {
        return _chirpDbContext.Cheeps.Count();
    }
    
    public int GetAuthorCheepCount(string authorLogin)
    {
        return _chirpDbContext.Cheeps.Count(c => c.Author.Login == authorLogin);
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
                        AuthorLogin = c.Author.Login,
                        Text = c.Text,
                        Timestamp = c.Timestamp
                    }
                )
                .ToList();
        });
    }

    public List<CheepDto> GetAuthorCheepsForPage(string authorLogin, int pageNumber)
    {
        return FetchWithErrorHandling(() =>
        { 
            return _chirpDbContext
                .Cheeps
                .Where(c => c.Author.Name == authorLogin)
                .Include(c => c.Author)
                .OrderByDescending(c => c.Timestamp)
                .Skip(int.Max(pageNumber - 1, 0) * 32)
                .Take(32)
                .Select<Cheep, CheepDto>(c =>
                    new CheepDto {
                        CheepId = c.CheepId,
                        AuthorName = c.Author.Name,
                        AuthorLogin = c.Author.Login,
                        Text = c.Text,
                        Timestamp = c.Timestamp
                    }
                )
                .ToList();
        });
    }

    public List<CheepDto> GetAuthorCheepsForPageAsOwner(string authorLogin, int pageNumber)
    {
        return FetchWithErrorHandling(() =>
        {
            var login = _chirpDbContext.Authors.Single(a => a.Login == authorLogin).Login;
            List<string> authorFollows = _authorRepository.GetFollowsForAuthor(login);
            return _chirpDbContext
                .Cheeps
                .Where(c => authorFollows.Contains(c.Author.Login))
                .Include(c => c.Author)
                .OrderByDescending(c => authorFollows.Contains(c.Author.Login))
                .ThenBy(c => c.Timestamp)
                .Skip(int.Max(pageNumber - 1, 0) * 32)
                .Take(32)
                .Select<Cheep, CheepDto>(c =>
                    new CheepDto {
                        CheepId = c.CheepId,
                        AuthorName = c.Author.Name,
                        AuthorLogin = c.Author.Login,
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