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

    public async Task AddAuthor(AuthorDto authorDto)
    {
        var authorWithIdExists = await _chirpDbContext.Authors.AnyAsync(a => a.AuthorId == authorDto.Id);
        
        if (!authorWithIdExists)
        {
            _chirpDbContext.Authors.Add(new Author
            {
                AuthorId = authorDto.Id,
                Name = authorDto.Name,
                Username = authorDto.Username,
                AvatarUrl = authorDto.AvatarUrl
            });
            await _chirpDbContext.SaveChangesAsync();
        }
    }
    
    public async Task<List<string>> GetFollowsForAuthor(Guid authorId)
    {
        Author? author = await _chirpDbContext.Authors
            .Include(a => a.Follows)
            .FirstOrDefaultAsync(a => a.AuthorId == authorId);
        
        if (author == null) return new List<string>();

        return GetFollows(author);
    }
    
    public async Task<List<string>> GetFollowsForAuthor(string authorUsername)
    {
        Author? author = await _chirpDbContext.Authors
            .Include(a => a.Follows)
            .FirstOrDefaultAsync(a => a.Username == authorUsername);
        
        if (author == null) return new List<string>();

        return GetFollows(author);
    }

    private List<string> GetFollows(Author author)
    {
        List<string> followsUsernames = new List<string>();
        
        author.Follows.ForEach(
            a => followsUsernames.Add(a.Username)
        );

        return followsUsernames;
    }

    public async Task AddFollow(Guid authorId, Guid followId)
    {
        Author? userAuthor = await _chirpDbContext.Authors.FirstOrDefaultAsync(a => a.AuthorId == authorId);
        Author? followAuthor = await _chirpDbContext.Authors.FirstOrDefaultAsync(a => a.AuthorId == followId);

        if (userAuthor == null) return;
        if (followAuthor == null) return;

        _chirpDbContext.Authors.UpdateRange(userAuthor, followAuthor);
        
        userAuthor.Follows.Add(followAuthor);

        followAuthor.FollowedBy.Add(userAuthor);

        await _chirpDbContext.SaveChangesAsync();
    }

    public async Task RemoveFollow(Guid authorId, Guid unfollowId)
    {
        Author? userAuthor = await _chirpDbContext.Authors.FirstOrDefaultAsync(a => a.AuthorId == authorId);
        Author? unfollowAuthor = await _chirpDbContext.Authors.FirstOrDefaultAsync(a => a.AuthorId == unfollowId);
        if (userAuthor == null) return;
        if (unfollowAuthor == null) return;
            
        _chirpDbContext.Authors.UpdateRange(userAuthor, unfollowAuthor);
            
        userAuthor.Follows.Remove(unfollowAuthor);

        unfollowAuthor.FollowedBy.Remove(userAuthor);

        await _chirpDbContext.SaveChangesAsync();
    }

    public async Task<AuthorDto?> GetAuthorFromUsername(string authorUsername)
    {
        Author? author = await _chirpDbContext.Authors.FirstOrDefaultAsync(a => a.Username == authorUsername);
        
        if (author == null) return null;
        
        return new AuthorDto
        {
            Id = author.AuthorId,
            Username = author.Username,
            Name = author.Name,
            AvatarUrl = author.AvatarUrl
        };
    }

    public async Task<bool> DeleteAuthor(Guid authorId)
    {
        return await WithErrorHandlingDefaultValueAsync(false, async () =>
        {
            Author? author = await _chirpDbContext.Authors.Include(a => a.Likes).Include(a => a.Comments).FirstOrDefaultAsync(a => a.AuthorId == authorId);
            if (author is null) throw new NullReferenceException("Author not found");

            // Delete user likes
            _chirpDbContext.Likes.RemoveRange(author.Likes);
            // Delete user comments
            _chirpDbContext.Comments.RemoveRange(author.Comments);
            // Delete user
            _chirpDbContext.Authors.Remove(author);

            await _chirpDbContext.SaveChangesAsync();
            return true;
        });
    }

    private async Task<T> WithErrorHandlingDefaultValueAsync<T>(T defaultValue, Func<Task<T>> function) where T : struct
    {
        try
        {
            return await function();
        }
        catch
        {
            return defaultValue;
        }
    }
}