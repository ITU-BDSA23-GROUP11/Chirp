using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Contexts;
using Chirp.Infrastructure.Models;

namespace Chirp.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly ChirpDbContext _chirpDbContext;

    public CommentRepository(ChirpDbContext chirpDbContext)
    {
        _chirpDbContext = chirpDbContext;
    }
    
    //Add a comment to a Cheep
    public bool AddComment(AddCommentDto commentDto)
    {
        Cheep? cheep = _chirpDbContext.Cheeps
            .SingleOrDefault(c => c.CheepId == commentDto.CheepId);

        if (cheep == null) return false;
        
        //Build the comment
        Comment comment = new Comment
        {
            CommentId = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            Text = commentDto.Text,
            Cheep = _chirpDbContext.Cheeps.Single(c => c.CheepId == commentDto.CheepId),
            CommentAuthor = _chirpDbContext.Authors.Single(a => a.AuthorId == commentDto.AuthorId)
        };

        _chirpDbContext.Comments.Add(comment);
        _chirpDbContext.SaveChanges();
        
        return true;
    }
    
    //Delete a comment from a Cheep
    //TODO: Implement fetch with error handling?
    public bool DeleteComment(Guid commentId)
    {
        try
        {
            var comment = _chirpDbContext.Comments.First(c => c.CommentId == commentId);
            _chirpDbContext.Remove(comment);
            _chirpDbContext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            //If the comment has already been deleted -> return false
            return false;
        }
    }
}