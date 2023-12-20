using Chirp.Core.Repositories;
using Chirp.Core.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.WebService.Controllers
{
    public class CommentController : BaseController
    {
        public CommentController(IAuthorRepository authorRepository, ICheepRepository cheepRepository, ILikeRepository likeRepository, ICommentRepository commentRepository) : base(authorRepository, cheepRepository, likeRepository, commentRepository)
        {
        }
        
        //POST: Comment/Add
        [HttpPost]
        [Route("Comment/Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(IFormCollection collection)
        {
            return await WithAuthAsync(async user =>
            {
                String commentString = collection["CommentText"].ToString();
                Guid cheepId = Guid.Parse(collection["CheepId"].ToString());
                Guid authorId = Guid.Parse(collection["AuthorId"].ToString());
                
                AddCommentDto addCommentDto = new AddCommentDto
                {
                    AuthorId = authorId,
                    CheepId = cheepId,
                    Text = commentString
                };
                
                await CommentRepository.AddComment(addCommentDto);
                
                return Redirect(GetPathUrl());
            });
        }
        
        //POST: Comment/Remove
        [HttpPost]
        [Route("Comment/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(IFormCollection collection)
        {
            return await WithAuthAsync(async _ =>
            {
                Guid commentId = Guid.Parse(collection["CommentId"].ToString());
                await CommentRepository.DeleteComment(commentId);

                return Redirect(GetPathUrl());
            });
        }
    }
}