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
        Author? author = _chirpDbContext.Authors.FirstOrDefault(a => a.AuthorId == cheep.AuthorId);
        
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
            AuthorId = newCheep.Author.AuthorId,
            AuthorName = newCheep.Author.Name,
            AuthorUsername = newCheep.Author.Username,
            AuthorAvatarUrl = newCheep.Author.AvatarUrl,
            Text = newCheep.Text,
            Timestamp = newCheep.Timestamp
        };
    }
    
    public int GetCheepCount()
    {
        return _chirpDbContext.Cheeps.Count();
    }
    
    public int GetAuthorCheepCount(string authorUsername, Guid? authUser = null)
    {
        int cheepCount = _chirpDbContext.Cheeps.Count(c => c.Author.Username == authorUsername);
        
        if (authUser is not null)
        {
            List<string> follows = _authorRepository.GetFollowsForAuthor((Guid)authUser);
            cheepCount += _chirpDbContext
                .Cheeps
                .Include(c => c.Author)
                .Count(c => follows.Contains(c.Author.Username));
        }
        
        return cheepCount;
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
                        AuthorId = c.Author.AuthorId,
                        AuthorName = c.Author.Name,
                        AuthorUsername = c.Author.Username,
                        AuthorAvatarUrl = c.Author.AvatarUrl,
                        Text = c.Text,
                        Timestamp = c.Timestamp
                    }
                )
                .ToList();
        });
    }

    public List<CheepDto> GetCheepsFromIds(HashSet<Guid> cheepIds)
    {
        return FetchWithErrorHandling(() =>
        {
            return _chirpDbContext.Cheeps
                .Include(c => c.Author)
                .Where(c => cheepIds.Contains(c.CheepId))
                .Select<Cheep, CheepDto>(c =>
                    new CheepDto
                    {
                        CheepId = c.CheepId,
                        AuthorId = c.Author.AuthorId,
                        AuthorName = c.Author.Name,
                        AuthorUsername = c.Author.Username,
                        AuthorAvatarUrl = c.Author.AvatarUrl,
                        Text = c.Text,
                        Timestamp = c.Timestamp
                    })
                .ToList();
        });
    }

    public List<CheepDto> GetAuthorCheepsForPage(string authorUsername, int pageNumber)
    {
        return FetchWithErrorHandling(() =>
        { 
            return _chirpDbContext
                .Cheeps
                .Where(c => c.Author.Username == authorUsername)
                .Include(c => c.Author)
                .OrderByDescending(c => c.Timestamp)
                .Skip(int.Max(pageNumber - 1, 0) * 32)
                .Take(32)
                .Select<Cheep, CheepDto>(c =>
                    new CheepDto {
                        CheepId = c.CheepId,
                        AuthorId = c.Author.AuthorId,
                        AuthorName = c.Author.Name,
                        AuthorUsername = c.Author.Username,
                        AuthorAvatarUrl = c.Author.AvatarUrl,
                        Text = c.Text,
                        Timestamp = c.Timestamp
                    }
                )
                .ToList();
        });
    }

    public List<CheepDto> GetAuthorCheepsForPageAsOwner(Guid authorId, int pageNumber)
    {
        return FetchWithErrorHandling(() =>
        {
            List<string> authorFollows = _authorRepository.GetFollowsForAuthor(authorId);
            return _chirpDbContext
                .Cheeps
                .Where(c => authorFollows.Contains(c.Author.Username) || c.Author.AuthorId.ToString().Equals(authorId.ToString()))
                .Include(c => c.Author)
                .OrderByDescending(c => authorFollows.Contains(c.Author.Username))
                .ThenBy(c => c.Timestamp)
                .Skip(int.Max(pageNumber - 1, 0) * 32)
                .Take(32)
                .Select<Cheep, CheepDto>(c =>
                    new CheepDto {
                        CheepId = c.CheepId,
                        AuthorId = c.Author.AuthorId,
                        AuthorName = c.Author.Name,
                        AuthorUsername = c.Author.Username,
                        AuthorAvatarUrl = c.Author.AvatarUrl,
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
        
        _chirpDbContext.Cheeps.Remove(cheepToDelete);
        _chirpDbContext.SaveChanges();
        
        return true; 
    }
}