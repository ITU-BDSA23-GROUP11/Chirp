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

    public void LikeCheep(Guid authorId, Guid cheepId) //Creates a like in DbContext
    {
        if (IsLiked(authorId, cheepId))
        {
            return;
        }

        _chirpDbContext.Likes.Add(new Like
        {
            LikedByAuthorId = authorId,
            CheepId = cheepId
        });
        _chirpDbContext.SaveChanges();
    }

    public void UnlikeCheep(Guid authorId, Guid cheepId) //Removes a like from DbContext
    {
        if (IsLiked(authorId, cheepId))
        {
            var toUnlike = _chirpDbContext.Likes.First(x => x.LikedByAuthorId == authorId && x.CheepId == cheepId);
            _chirpDbContext.Remove(toUnlike);
            _chirpDbContext.SaveChanges();
        }
    }

    public int LikeCount(Guid cheepId) //Counts amount of likes of a cheep
    {
        return _chirpDbContext.Likes.Count(x => x.CheepId == cheepId);
    }

    public List<LikeDto> GetLikesByAuthorId(Guid authorId) //A list of an authors likes
    {
        return _chirpDbContext.Likes
            .Where(l => l.LikedByAuthorId == authorId)
            .Select<Like, LikeDto>(l =>
                new LikeDto
                {
                    CheepId = l.CheepId,
                    LikedByAuthorId = l.LikedByAuthorId
                }
            ).ToList();
    }
   

    public List<LikeDto> GetLikesByCheepId(Guid cheepId) //A list of a cheeps likes
    {
        return _chirpDbContext.Likes
            .Where(l => l.CheepId == cheepId)
            .Select<Like, LikeDto>(l =>
                new LikeDto
                {
                    CheepId = l.CheepId,
                    LikedByAuthorId = l.LikedByAuthorId
                }
            ).ToList();
    }

    public bool IsLiked(Guid authorId, Guid cheepId) //Checks if a cheep is already liked
    {
        if (!_chirpDbContext.Likes.Any(x => x.LikedByAuthorId == authorId && x.CheepId == cheepId))
        {
            return false;
        } 
        return _chirpDbContext.Likes.Any(x => x.LikedByAuthorId == authorId && x.CheepId == cheepId);
    }

}