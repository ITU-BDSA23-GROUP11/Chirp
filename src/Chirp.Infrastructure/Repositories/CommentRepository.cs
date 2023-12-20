using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Contexts;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly ChirpDbContext _chirpDbContext;

    public CommentRepository(ChirpDbContext chirpDbContext)
    {
        _chirpDbContext = chirpDbContext;
    }
    
    //Add a comment to a Cheep
    public async Task<bool> AddComment(AddCommentDto commentDto)
    {
        Cheep? cheep = await _chirpDbContext.Cheeps
            .SingleOrDefaultAsync(c => c.CheepId == commentDto.CheepId);

        if (cheep == null) return false;
        
        //Build the comment
        Comment comment = new Comment
        {
            CommentId = Guid.NewGuid(),
            Text = commentDto.Text,
            Cheep = _chirpDbContext.Cheeps.Single(c => c.CheepId == commentDto.CheepId),
            CommentAuthor = _chirpDbContext.Authors.Single(a => a.AuthorId == commentDto.AuthorId)
        };

        _chirpDbContext.Comments.Add(comment);
        await _chirpDbContext.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> DeleteComment(Guid commentId)
    {
        var comment = await _chirpDbContext.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);
        if (comment is null) return false;
        _chirpDbContext.Remove(comment);
        await _chirpDbContext.SaveChangesAsync();
        return true;
    }
}