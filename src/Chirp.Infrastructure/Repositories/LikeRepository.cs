using Chirp.Core.Repositories;
using Chirp.Infrastructure.Contexts;
using Chirp.Infrastructure.Models;

namespace Chirp.Infrastructure.Repositories;

/*Like repository to implement likes in cheeps*/

public class LikeRepository : ILikeRepository
{
    private readonly ChirpDbContext _chirpDbContext;

    public LikeRepository(ChirpDbContext chirpDbContext)
    {
        _chirpDbContext = chirpDbContext;
    }

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

    public void UnlikeCheep(Guid authorId, Guid cheepId)
    {
        if (IsLiked(authorId, cheepId))
        {
            var toUnlike = _chirpDbContext.Likes.First(x => x.LikedByAuthorId == authorId && x.CheepId == cheepId);
            _chirpDbContext.Remove(toUnlike);
            _chirpDbContext.SaveChanges();
        }
    }

    public int LikeCount(Guid cheepId)
    {
        return _chirpDbContext.Likes.Count(x => x.CheepId == cheepId);
    }

    public bool IsLiked(Guid authorId, Guid cheepId)
    {
        return _chirpDbContext.Likes.Any(x => x.LikedByAuthorId == authorId && x.CheepId == cheepId);
    }

}