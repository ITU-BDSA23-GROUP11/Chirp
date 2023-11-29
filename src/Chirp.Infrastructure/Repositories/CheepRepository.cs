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
            AuthorEmail = newCheep.Author.Email,
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
        List<string> authorFollows = GetFollowsForAuthor(GetAuthorEmailByName(authorName));
        return FetchWithErrorHandling(() =>
        {
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

    public List<string> GetFollowsForAuthor(string authorEmail)
    {
        Author author = _chirpDbContext.Authors.Include(a => a.Follows).FirstOrDefault(a => a.Email == authorEmail);
        if (author == null) return new List<string>();
        
        List<string> followsEmails = new List<string>();
        
        author.Follows.ForEach(
            a => followsEmails.Add(a.Email)
        );

        return followsEmails;
    }

    public void AddFollow(string authorEmail, string followEmail)
    {
        Author? userAuthor = _chirpDbContext.Authors.FirstOrDefault(a => a.Email == authorEmail);

        if (userAuthor == null) return;

        Author? followAuthor = _chirpDbContext.Authors.FirstOrDefault(a => a.Email == followEmail);

        if (followAuthor == null) return;

        _chirpDbContext.Authors.UpdateRange(userAuthor, followAuthor);
        
        userAuthor.Follows.Add(followAuthor);

        followAuthor.FollowedBy.Add(userAuthor);

        _chirpDbContext.SaveChanges();
    }

    public void RemoveFollow(string authorEmail, string unfollowEmail)
    {
        Author? userAuthor = _chirpDbContext.Authors.Include(a => a.Follows).FirstOrDefault(a => a.Email == authorEmail);

        if (userAuthor == null) return;

        Author? unfollowAuthor = _chirpDbContext.Authors.Include(a => a.FollowedBy).FirstOrDefault(a => a.Email == unfollowEmail);

        if (unfollowAuthor == null) return;
            
        _chirpDbContext.Authors.UpdateRange(userAuthor, unfollowAuthor);
            
        userAuthor.Follows.Remove(unfollowAuthor);

        unfollowAuthor.FollowedBy.Remove(userAuthor);

        _chirpDbContext.SaveChanges();
    }

    public string GetAuthorEmailByName(string authorName)
    {
        string? email = _chirpDbContext.Authors.Single(a => a.Name == authorName).Email;
        return email;
    }

    public string GetAuthorNameByEmail(string authorEmail)
    {
        string name = _chirpDbContext.Authors.Single(a => a.Email == authorEmail).Name;
        if (name == null) throw new Exception("Could not find name in database");
        return name;
    }
}