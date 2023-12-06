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
                Login = authorDto.Login
            });
            _chirpDbContext.SaveChanges();
        }
    }
    
    public List<string> GetFollowsForAuthor(string authorLogin)
    {
        Author? author = _chirpDbContext.Authors
            .Include(a => a.Follows)
            .FirstOrDefault(a => a.Login == authorLogin);
        
        if (author == null) return new List<string>();
        
        List<string> followsLogins = new List<string>();
        
        author.Follows.ForEach(
            a => followsLogins.Add(a.Login)
        );

        return followsLogins;
    }

    public void AddFollow(string authorLogin, string followLogin)
    {
        Author? userAuthor = _chirpDbContext.Authors.FirstOrDefault(a => a.Login == authorLogin);

        if (userAuthor == null) return;

        Author? followAuthor = _chirpDbContext.Authors.FirstOrDefault(a => a.Login == followLogin);

        if (followAuthor == null) return;

        _chirpDbContext.Authors.UpdateRange(userAuthor, followAuthor);
        
        userAuthor.Follows.Add(followAuthor);

        followAuthor.FollowedBy.Add(userAuthor);

        _chirpDbContext.SaveChanges();
    }

    public void RemoveFollow(string authorLogin, string unfollowLogin)
    {
        Author? userAuthor = _chirpDbContext.Authors.Include(a => a.Follows).FirstOrDefault(a => a.Login == authorLogin);

        if (userAuthor == null) return;

        Author? unfollowAuthor = _chirpDbContext.Authors.Include(a => a.FollowedBy).FirstOrDefault(a => a.Login == unfollowLogin);

        if (unfollowAuthor == null) return;
            
        _chirpDbContext.Authors.UpdateRange(userAuthor, unfollowAuthor);
            
        userAuthor.Follows.Remove(unfollowAuthor);

        unfollowAuthor.FollowedBy.Remove(userAuthor);

        _chirpDbContext.SaveChanges();
    }
}