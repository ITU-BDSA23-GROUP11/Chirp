using Chirp.Core.Dto;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Contexts;
using Microsoft.Identity.Client;

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
    
    public bool DeleteCheep(String cheepId, String author)
    {
        Cheep cheepToDelete = _chirpDbContext.Cheeps
            .Include(c => c.Author)
            .First(c => c.CheepId.ToString() == cheepId);
        if (!cheepToDelete.Author.Name.Equals(author))
        {
            return false;
        }
        
        _chirpDbContext.Cheeps.Remove(cheepToDelete);
        _chirpDbContext.SaveChanges();
        
        return true; 
    }

    public List<string> GetFollowsForAuthor(string authorEmail)
    {
        Author author = _chirpDbContext.Authors.Include(a => a.Follows).FirstOrDefault(a => a.Email == authorEmail);
        if (author == null) throw new Exception("The author could not be found");
        
        List<string> followsEmails = new List<string>();
        
        author.Follows.ForEach(
            a => followsEmails.Add(a.Email)
        );

        return followsEmails;
    }

    public void AddFollow(string authorEmail, string followEmail)
    {
        try
        {
            
            Author? userAuthor = _chirpDbContext.Authors.FirstOrDefault(a => a.Email == authorEmail);

            if (userAuthor == null) throw new Exception("User could not be found...");

            Author? followAuthor = _chirpDbContext.Authors.FirstOrDefault(a => a.Email == followEmail);

            if (followAuthor == null) throw new Exception("Could not find user to be followed");

            _chirpDbContext.Authors.UpdateRange(userAuthor, followAuthor);
            
            userAuthor.Follows.Add(followAuthor);

            followAuthor.FollowedBy.Add(userAuthor);

            _chirpDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            
        }
    }

    public void RemoveFollow(string authorEmail, string unfollowEmail)
    {
        try
        {
            
            Author? userAuthor = _chirpDbContext.Authors.FirstOrDefault(a => a.Email == authorEmail);

            if (userAuthor == null) throw new Exception("User could not be found...");

            Author? unfollowAuthor = _chirpDbContext.Authors.FirstOrDefault(a => a.Email == unfollowEmail);

            if (unfollowAuthor == null) throw new Exception("Could not find user to be followed");

            _chirpDbContext.Authors.UpdateRange(userAuthor, unfollowAuthor);
            
            userAuthor.Follows.Add(unfollowAuthor);

            unfollowAuthor.FollowedBy.Add(userAuthor);

            _chirpDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            
        }
    }
}