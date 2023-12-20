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
    public async Task LikeCheep(Guid authorId, Guid cheepId) 
    {
        Author? author = await _chirpDbContext
            .Authors
            .Include(a => a.Likes)
            .ThenInclude(like => like.Cheep)
            .SingleOrDefaultAsync(a => a.AuthorId == authorId);
        
        Cheep? cheep = await _chirpDbContext.Cheeps.Include(c => c.Likes).SingleOrDefaultAsync(c => c.CheepId == cheepId);

        if (author is null) return;
        if (cheep is null) return;
        if (author.Likes.Any(l => l.Cheep.CheepId.ToString() == cheepId.ToString())) return; // Already liked

        _chirpDbContext.Likes.Add(new Like
        {
            LikedByAuthor = author,
            Cheep = cheep
        });
        
        await _chirpDbContext.SaveChangesAsync();
    }
    //Removes a like from DbContext
    public async Task UnlikeCheep(Guid authorId, Guid cheepId) 
    {
        Like? like = await _chirpDbContext.Likes.SingleOrDefaultAsync(c => c.Cheep.CheepId == cheepId && c.LikedByAuthor.AuthorId == authorId);
        if (like is null) return;//Not liked

        _chirpDbContext.Likes.Remove(like);
        await _chirpDbContext.SaveChangesAsync();
    }
    
    //A list of an authors likes
    public async Task<List<LikeDto>> GetLikesByAuthorId(Guid authorId) 
    {
        return await _chirpDbContext
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
            ).ToListAsync();
    }


}