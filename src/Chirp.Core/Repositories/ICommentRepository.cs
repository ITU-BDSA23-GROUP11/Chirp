using Chirp.Core.Dto;

namespace Chirp.Core.Repositories;

public interface ICommentRepository
{
    public CommentDto? AddCommentDto(AddCommentDto commentDto); //Add a comment to a cheep
    public List<CommentDto>GetCommentsForCheep(int amountOfComments); //Show a specific amount of cheeps underneath a cheep
    public bool DeleteComment(Guid commentId); //Deletes a comment from the database
}