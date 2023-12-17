using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Contexts;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly ChirpDbContext _chirpDbContext;
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;

    public CommentRepository(ChirpDbContext chirpDbContext, ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _chirpDbContext = chirpDbContext;
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
    }
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
}