using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Contexts;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpDbContext _chirpDbContext;

    public AuthorRepository(ChirpDbContext chirpDbContext)
    {
        _chirpDbContext = chirpDbContext;
    }

    public void AddAuthor(AuthorDto authorDto)
    {
        var authorWithIdExists = _chirpDbContext.Authors.Any(a => a.AuthorId == authorDto.Id);
        
        if (!authorWithIdExists)
        {
            _chirpDbContext.Authors.Add(new Author
            {
                AuthorId = authorDto.Id,
                Name = authorDto.Name,
                Email = authorDto.Email
            });
            _chirpDbContext.SaveChanges();
        }
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

    public string GetAuthorNameByEmail(string authorEmail)
    {
        string name = _chirpDbContext.Authors.Single(a => a.Email == authorEmail).Name;
        if (name == null) throw new Exception("Could not find name in database");
        return name;
    }
}