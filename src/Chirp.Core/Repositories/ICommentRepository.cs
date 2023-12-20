using Chirp.Core.Dto;

namespace Chirp.Core.Repositories;

public interface ICommentRepository
{
    public Task<bool> AddComment(AddCommentDto commentDto);
    public Task<bool> DeleteComment(Guid commentId);
}