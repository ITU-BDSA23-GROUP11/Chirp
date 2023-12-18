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
    
    /*
    //Add a comment to a cheep
    public CommentDto? AddCommentDto(AddCommentDto comment)
    {
        Author? author = _chirpDbContext.Authors.FirstOrDefault(a => a.AuthorId == comment.AuthorId);

        if (author == null) return null;

        Comment newComment = new Comment
        {
            Author = author,
            Text = comment.Text,
            Timestamp = DateTime.UtcNow
        };
        _chirpDbContext.Comments.Add(newComment);
        _chirpDbContext.SaveChanges();

        return new CommentDto
        {
            CommentId = newComment.CommentId,
            CheepId = newComment.Cheep.CheepId,
            AuthorId = newComment.Author.AuthorId,
            AuthorName = newComment.Author.Name,
            AuthorAvatarUrl = newComment.Author.AvatarUrl,
            Text = newComment.Text,
            Timestamp = newComment.Timestamp
            
        };
    } 

    //Show a specific amount of cheeps underneath a cheep
    public List<CommentDto> GetCommentsForCheep(int amountOfComments)
    {
        return FetchWithErrorHandling(() =>
        {
            return _chirpDbContext
                .Comments
                .Include(c => c.Author)
                .OrderByDescending(c => c.Timestamp)
                .Skip(int.Max(amountOfComments - 1, 0) * 5).Take(5)
                .Select<Comment, CommentDto>(c =>
                    new CommentDto
                    {
                        CommentId = c.CommentId,
                        AuthorId = c.Author.AuthorId,
                        AuthorName = c.Author.Name,
                        AuthorAvatarUrl = c.Author.AvatarUrl,
                        CheepId = c.Cheep.CheepId,
                        Text = c.Text,
                        Timestamp = c.Timestamp
                    }
                ).ToList();
        });
    } 

    //Deletes a comment from the database
    public bool DeleteComment(Guid commentId)
    {
        Comment? commentToDelete = _chirpDbContext
            .Comments
            .SingleOrDefault(c => c.CommentId == commentId);
        if (commentToDelete == null) return false;

        _chirpDbContext.Comments.Remove(commentToDelete);
        _chirpDbContext.SaveChanges();

        return true;
    } 
    
    private List<CommentDto> FetchWithErrorHandling(Func<List<CommentDto>> fetchFunction)
    {
        try
        {
            return fetchFunction();
        }
        catch
        {
            return new List<CommentDto>();
        }
    }
    */
}