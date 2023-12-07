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
    //Removes a like from DbContext
    public void UnlikeCheep(Guid authorId, Guid cheepId) 
    {
        if (IsLiked(authorId, cheepId))
        {
            var toUnlike = _chirpDbContext.Likes.First(x => x.LikedByAuthorId == authorId && x.CheepId == cheepId);
            _chirpDbContext.Remove(toUnlike);
            _chirpDbContext.SaveChanges();
        }
    }
    //Counts amount of likes of a cheep
    public int LikeCount(Guid cheepId) 
    {
        return _chirpDbContext.Likes.Count(x => x.CheepId == cheepId);
    }
    //A list of an authors likes
    public List<LikeDto> GetLikesByAuthorId(Guid authorId) 
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
   
    //A list of a cheeps likes
    public List<LikeDto> GetLikesByCheepId(Guid cheepId) 
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
    //Finds a specific like based on authorId and cheepId
    public LikeDto GetLike(Guid authorId, Guid cheepId) 
    {
        return _chirpDbContext.Likes
            .Where(l => l.LikedByAuthorId == authorId)
            .Where(l => l.CheepId == cheepId)
            .Select<Like, LikeDto>(l => new LikeDto
            {
                CheepId = l.CheepId,
                LikedByAuthorId = l.LikedByAuthorId
            }
            ).ToList().First();
    }
    //Checks if a like exists
    public bool IsLiked(Guid authorId, Guid cheepId) 
    {
        _chirpDbContext.Likes.Any(x => x.LikedByAuthorId == authorId && x.CheepId == cheepId);
       
    }

}
