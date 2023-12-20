using Chirp.Core.Repositories;
using Chirp.Core.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.WebService.Controllers
{
    public class CheepController : BaseController
    {
        public CheepController(IAuthorRepository authorRepository, ICheepRepository cheepRepository, ILikeRepository likeRepository, ICommentRepository commentRepository) : base(authorRepository, cheepRepository, likeRepository, commentRepository)
        {
        }
        
        // POST: Cheep/Like
        [HttpPost]
        [Route("Cheep/Like")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Like(IFormCollection collection)
        {
            return await WithAuthAsync(async user =>
            {
                String? cheepId = collection["cheepId"];
                if (String.IsNullOrEmpty(cheepId))
                {
                    return RedirectWithError("Invalid input");
                }
                Guid cId = Guid.Parse(cheepId);
                await LikeRepository.LikeCheep(user.Id, cId);
                
                return Redirect(GetPathUrl());
            });
        }
        
        //POST: Unlike cheep
        [HttpPost]
        [Route("Cheep/Unlike")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlike(IFormCollection collection)
        {
            return await WithAuthAsync(async user =>
            {
                String? cheepId = collection["cheepId"];
                if (String.IsNullOrEmpty(cheepId))
                {
                    return RedirectWithError("Invalid input");
                }
                Guid cId = Guid.Parse(cheepId);
                await LikeRepository.UnlikeCheep(user.Id, cId);
                
                return Redirect(GetPathUrl());
            });
        }
        

        // POST: Cheep/Create
        [HttpPost]
        [Route("Cheep/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            return await WithAuthAsync(async user =>
            {
                string? cheepText = collection["cheepText"];

                if (String.IsNullOrEmpty(cheepText))
                {
                    return RedirectWithError("Invalid input");
                }

                if (cheepText.Length > 160)
                {
                    return RedirectWithError("Invalid input - cheep is too long (max 160 characters)");
                }

                await CheepRepository.AddCheep(new AddCheepDto
                {
                    AuthorId = user.Id,
                    Text = cheepText
                });
                return Redirect(GetPathUrl());
            });
        }
        
        // POST: Cheep/Delete
        [HttpPost]
        [Route("Cheep/Delete")]
        [ValidateAntiForgeryToken]
        //public IActionResult Delete(Guid id)
        public async Task<IActionResult> Delete(IFormCollection collection)
        {
            return await WithAuthAsync(async user =>
            {
                string? cheepId = collection["cheepId"];
                
                if (String.IsNullOrEmpty(cheepId))
                {
                    return RedirectWithError("Invalid Cheep Id");
                }
                
                if (!await CheepRepository.DeleteCheep(Guid.Parse(cheepId))) return RedirectWithError("Cheep was not found");
                
                return Redirect(GetPathUrl());
            });
        }
        
        //Post Cheep/Follow
        [HttpPost]
        [Route("Cheep/Follow")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Follow(IFormCollection collection)
        {
            return await WithAuthAsync(async user =>
            {
                //The new account to follow
                Guid authorToBeFollowed = Guid.Parse(collection["CheepAuthorId"].ToString());
                
                await AuthorRepository.AddFollow(user.Id, authorToBeFollowed);
                
                return Redirect(GetPathUrl());//Redirect to same page
            });
        }
        
        //Post Cheep/Unfollow
        [HttpPost]
        [Route("Cheep/Unfollow")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unfollow(IFormCollection collection)
        {
            return await WithAuthAsync(async user =>
            {
                Guid authorToBeUnfollowed = Guid.Parse(collection["CheepAuthorId"].ToString());//The new account to follow
                await AuthorRepository.RemoveFollow(user.Id, authorToBeUnfollowed);
                return Redirect(GetPathUrl());//Redirect to same page
            });
        }
    }
}