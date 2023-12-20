using Chirp.Core.Dto;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Contexts;

namespace Chirp.Infrastructure.Repositories;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDbContext _chirpDbContext;
    private readonly IAuthorRepository _authorRepository;

    public CheepRepository(ChirpDbContext chirpDbContext, IAuthorRepository authorRepository)
    {
        _chirpDbContext = chirpDbContext;
        _authorRepository = authorRepository;
    }

    public async Task<CheepDto?> AddCheep(AddCheepDto cheep)
    {
        Author? author = await _chirpDbContext.Authors.FirstOrDefaultAsync(a => a.AuthorId == cheep.AuthorId);
        
        if (author == null) return null;

        Cheep newCheep = new Cheep
        {
            Author = author,
            Text = cheep.Text,
        };
        
        _chirpDbContext.Cheeps.Add(newCheep);
        await _chirpDbContext.SaveChangesAsync();

        return MapCheepToDto(newCheep);
    }
    
    public Task<int> GetCheepCount()
    {
        return _chirpDbContext.Cheeps.CountAsync();
    }
    
    public async Task<int> GetAuthorCheepCount(string authorUsername, Guid? authUser = null)
    {
        int cheepCount = await _chirpDbContext.Cheeps.CountAsync(c => c.Author.Username == authorUsername);
        
        if (authUser is not null)
        {
            List<string> follows = await _authorRepository.GetFollowsForAuthor((Guid)authUser);
            cheepCount += await _chirpDbContext
                .Cheeps
                .Include(c => c.Author)
                .CountAsync(c => follows.Contains(c.Author.Username));
        }
        
        return cheepCount;
    }
    
    public Task<List<CheepDto>> GetCheepsForPage(int pageNumber)
    {
        return FetchWithErrorHandlingAsync( () =>
        {
            return _chirpDbContext
                .Cheeps
                .Include(c => c.Author)
                .Include(c => c.Likes)
                .OrderByDescending(c => c.Timestamp)
                .Skip(int.Max(pageNumber - 1, 0) * 32)
                .Take(32)
                .Select<Cheep, CheepDto>(c => MapCheepToDto(c))
                .ToListAsync();
        });
    }

    public Task<List<CheepDto>> GetCheepsFromIds(HashSet<Guid> cheepIds)
    {
        return FetchWithErrorHandlingAsync( () =>
        {
            return _chirpDbContext.Cheeps
                .Include(c => c.Author)
                .Include(c => c.Likes)
                .Where(c => cheepIds.Contains(c.CheepId))
                .Select<Cheep, CheepDto>(c => MapCheepToDto(c))
                .ToListAsync();
        });
    }

    public Task<List<CheepDto>> GetAuthorCheepsForPage(string authorUsername, int pageNumber)
    {
        return FetchWithErrorHandlingAsync(() =>
        { 
            return _chirpDbContext
                .Cheeps
                .Where(c => c.Author.Username == authorUsername)
                .Include(c => c.Author)
                .Include(c => c.Likes)
                .OrderByDescending(c => c.Timestamp)
                .Skip(int.Max(pageNumber - 1, 0) * 32)
                .Take(32)
                .Select<Cheep, CheepDto>(c => MapCheepToDto(c))
                .ToListAsync();
        });
    }

    public Task<List<CheepDto>> GetAuthorCheepsForPageAsOwner(Guid authorId, int pageNumber)
    {
        return FetchWithErrorHandlingAsync(async () =>
        {
            List<string> authorFollows = await _authorRepository.GetFollowsForAuthor(authorId);
            return await _chirpDbContext
                .Cheeps
                .Where(c => authorFollows.Contains(c.Author.Username) || c.Author.AuthorId.ToString().Equals(authorId.ToString()))
                .Include(c => c.Author)
                .OrderByDescending(c => authorFollows.Contains(c.Author.Username))
                .ThenByDescending(c => c.Timestamp)
                .Skip(int.Max(pageNumber - 1, 0) * 32)
                .Take(32)
                .Select<Cheep, CheepDto>(c => MapCheepToDto(c))
                .ToListAsync();
        });
    }
    
    private async Task<List<CheepDto>> FetchWithErrorHandlingAsync(Func<Task<List<CheepDto>>> fetchFunction)
    {
        try
        {
            return await fetchFunction();
        }
        catch
        {
            return new List<CheepDto>();
        }
    }
    
    public async Task<bool> DeleteCheep(Guid cheepId)
    {
        Cheep? cheepToDelete = await _chirpDbContext.Cheeps
            .Include(c => c.Author)
            .Include(c => c.Likes)
            .Include(c => c.Comments)
            .SingleOrDefaultAsync(c => c.CheepId == cheepId);

        if (cheepToDelete == null) return false;
        
        //If the cheep has likes or comments, remove them before the cheep
        _chirpDbContext.Likes.RemoveRange(cheepToDelete.Likes);
        _chirpDbContext.Comments.RemoveRange(cheepToDelete.Comments);
        
        _chirpDbContext.Cheeps.Remove(cheepToDelete);
        await _chirpDbContext.SaveChangesAsync();
        
        return true; 
    }

    private static CheepDto MapCheepToDto(Cheep cheep) =>
        new () {
            CheepId = cheep.CheepId,
            AuthorId = cheep.Author.AuthorId,
            AuthorName = cheep.Author.Name,
            AuthorUsername = cheep.Author.Username,
            AuthorAvatarUrl = cheep.Author.AvatarUrl,
            Text = cheep.Text,
            Timestamp = cheep.Timestamp,
            LikeCount = cheep.Likes.Count,
            CommentDtos = cheep.Comments.OrderByDescending(com => com.Timestamp).Select<Comment, CommentDto>(com =>
                new CommentDto
                {
                    AuthorId = com.CommentAuthor.AuthorId,
                    CheepId = com.Cheep.CheepId,
                    CheepAuthorId = com.Cheep.Author.AuthorId,
                    AuthorName = com.CommentAuthor.Name,
                    AuthorUsername = com.CommentAuthor.Username,
                    AuthorAvatarUrl = com.CommentAuthor.AvatarUrl,
                    CommentId = com.CommentId,
                    Text = com.Text,
                    Timestamp = com.Timestamp
                }).ToList()
        };
}