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
                Login = authorDto.Login,
                AvatarUrl = authorDto.AvatarUrl
            });
            _chirpDbContext.SaveChanges();
        }
    }
    
    public List<string> GetFollowsForAuthor(Guid authorId)
    {
        Author? author = _chirpDbContext.Authors
            .Include(a => a.Follows)
            .FirstOrDefault(a => a.AuthorId == authorId);
        
        if (author == null) return new List<string>();

        return GetFollows(author);
    }
    
    public List<string> GetFollowsForAuthor(string authorLogin)
    {
        Author? author = _chirpDbContext.Authors
            .Include(a => a.Follows)
            .FirstOrDefault(a => a.Login == authorLogin);
        
        if (author == null) return new List<string>();

        return GetFollows(author);
    }

    private List<string> GetFollows(Author author)
    {
        List<string> followsLogins = new List<string>();
        
        author.Follows.ForEach(
            a => followsLogins.Add(a.Login)
        );

        return followsLogins;
    }

    public void AddFollow(Guid authorId, Guid followId)
    {
        Author? userAuthor = _chirpDbContext.Authors.FirstOrDefault(a => a.AuthorId == authorId);

        if (userAuthor == null) return;

        Author? followAuthor = _chirpDbContext.Authors.FirstOrDefault(a => a.AuthorId == followId);

        if (followAuthor == null) return;

        _chirpDbContext.Authors.UpdateRange(userAuthor, followAuthor);
        
        userAuthor.Follows.Add(followAuthor);

        followAuthor.FollowedBy.Add(userAuthor);

        _chirpDbContext.SaveChanges();
    }

    public void RemoveFollow(Guid authorId, Guid unfollowId)
    {
        Author? userAuthor = _chirpDbContext.Authors.Include(a => a.Follows).FirstOrDefault(a => a.AuthorId == authorId);

        if (userAuthor == null) return;

        Author? unfollowAuthor = _chirpDbContext.Authors.Include(a => a.FollowedBy).FirstOrDefault(a => a.AuthorId == unfollowId);

        if (unfollowAuthor == null) return;
            
        _chirpDbContext.Authors.UpdateRange(userAuthor, unfollowAuthor);
            
        userAuthor.Follows.Remove(unfollowAuthor);

        unfollowAuthor.FollowedBy.Remove(userAuthor);

        _chirpDbContext.SaveChanges();
    }

    public AuthorDto? GetAuthorFromLogin(string authorLogin)
    {
        Author? author = _chirpDbContext.Authors.FirstOrDefault(a => a.Login == authorLogin);
        
        if (author == null) return null;
        
        return new AuthorDto
        {
            Id = author.AuthorId,
            Login = author.Login,
            Name = author.Name,
            AvatarUrl = author.AvatarUrl
        };
    }
}