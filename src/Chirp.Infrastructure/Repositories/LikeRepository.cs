using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Contexts;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

/*Like repository to implement likes in cheeps*/

public class LikeRepository : ILikeRepository
{
    
    private readonly ChirpDbContext _chirpDbContext;

    public LikeRepository(ChirpDbContext chirpDbContext)
    {
        _chirpDbContext = chirpDbContext;
        
    }
    //Creates a like in DbContext
    public void LikeCheep(Guid authorId, Guid cheepId) 
    {
        Author? author = _chirpDbContext
            .Authors
            .Include(a => a.Likes)
            .ThenInclude(like => like.Cheep)
            .SingleOrDefault(a => a.AuthorId == authorId);
        Cheep? cheep = _chirpDbContext.Cheeps.Include(c => c.Likes).SingleOrDefault(c => c.CheepId == cheepId);
        if (author is null) return;
        if (cheep is null) return;
        if (author.Likes.Any(l => l.Cheep.CheepId == cheepId)) return; // Already liked

        _chirpDbContext.Likes.Add(new Like
        {
            LikedByAuthor = author,
            Cheep = cheep
        });
        _chirpDbContext.SaveChanges();
    }
    //Removes a like from DbContext
    public void UnlikeCheep(Guid authorId, Guid cheepId) 
    {
        Author? author = _chirpDbContext
            .Authors
            .Include(a => a.Likes)
            .ThenInclude(like => like.Cheep)
            .SingleOrDefault(a => a.AuthorId == authorId);
        Cheep? cheep = _chirpDbContext.Cheeps.Include(c => c.Likes).SingleOrDefault(c => c.CheepId == cheepId);
        if (author is null) return;
        if (cheep is null) return;

        Like? like = author.Likes.FirstOrDefault(l => l.Cheep.CheepId == cheepId);
        if (like is null) return; // Not liked

        _chirpDbContext.Likes.Remove(like);
        _chirpDbContext.SaveChanges();
    }
    //Counts amount of likes of a cheep
    public int LikeCount(Guid cheepId)
    {
        return _chirpDbContext
            .Likes
            .Include(l => l.Cheep)
            .Count(l => l.Cheep.CheepId == cheepId);
    }
    
    //A list of an authors likes
    public List<LikeDto> GetLikesByAuthorId(Guid authorId) 
    {
        return _chirpDbContext
            .Likes
            .Include(l => l.LikedByAuthor)
            .Include(l => l.Cheep)
            .Where(l => l.LikedByAuthor.AuthorId == authorId)
            .Select<Like, LikeDto>(l =>
                new LikeDto
                {
                    CheepId = l.Cheep.CheepId,
                    LikedByAuthorId = l.LikedByAuthor.AuthorId
                }
            ).ToList();
    }
   
    //A list of a cheeps likes
    public List<LikeDto> GetLikesByCheepId(Guid cheepId) 
    {
        return _chirpDbContext
            .Likes
            .Include(l => l.LikedByAuthor)
            .Include(l => l.Cheep)
            .Where(l => l.Cheep.CheepId == cheepId)
            .Select<Like, LikeDto>(l =>
                new LikeDto
                {
                    CheepId = l.Cheep.CheepId,
                    LikedByAuthorId = l.LikedByAuthor.AuthorId
                }
            ).ToList();
    }
    
    //Finds a specific like based on authorId and cheepId
    public LikeDto GetLike(Guid authorId, Guid cheepId)
    {
        return _chirpDbContext
            .Likes
            .Include(l => l.Cheep)
            .Include(l => l.LikedByAuthor)
            .Where(l => l.LikedByAuthor.AuthorId == authorId)
            .Where(l => l.Cheep.CheepId == cheepId)
            .Select<Like, LikeDto>(l => new LikeDto
                {
                    CheepId = l.Cheep.CheepId,
                    LikedByAuthorId = l.LikedByAuthor.AuthorId
                }
            ).First();
    }
    
    //Checks if a like exists
    public bool IsLiked(Guid authorId, Guid cheepId)
    {
        return _chirpDbContext
            .Likes
            .Include(l => l.Cheep)
            .Include(l => l.LikedByAuthor)
            .Where(l => l.LikedByAuthor.AuthorId == authorId)
            .Any(l => l.Cheep.CheepId == cheepId);
    }

}