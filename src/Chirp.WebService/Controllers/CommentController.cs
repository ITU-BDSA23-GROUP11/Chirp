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
        public IActionResult Add(IFormCollection collection)
        {
            return WithAuth(user =>
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
                
                CommentRepository.AddComment(addCommentDto);
                
                return Redirect(GetPathUrl());
            });
        }
        
        //POST: Comment/Remove
        [HttpPost]
        [Route("Comment/Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(IFormCollection collection)
        {
            return WithAuth(user =>
            {
                Guid commentId = Guid.Parse(collection["CommentId"].ToString());
                CommentRepository.DeleteComment(commentId);

                return Redirect(GetPathUrl());
            });
        }
    }
}