using Chirp.Core.Repositories;
using Chirp.Core.Dto;
using Chirp.WebService.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.WebService.Controllers
{
    public class CheepController : BaseController
    {
        public CheepController(IAuthorRepository authorRepository, ICheepRepository cheepRepository, ILikeRepository likeRepository) : base(authorRepository, cheepRepository, likeRepository)
        {
        }
        
        // POST: Cheep/Like
        [HttpPost]
        [Route("Cheep/Like")]
        [ValidateAntiForgeryToken]
        public IActionResult Like(IFormCollection collection)
        {
            return WithAuth(user =>
            {
                String? cheepId = collection["cheepId"];
                if (String.IsNullOrEmpty(cheepId))
                {
                    return BadRequest("Invalid input");
                }
                Guid cId = Guid.Parse(cheepId);
                LikeRepository.LikeCheep(user.Id, cId);
                
                return Redirect(GetPathUrl());
            });
        }
        
        //POST: Unlike cheep
        [HttpPost]
        [Route("Cheep/Unlike")]
        [ValidateAntiForgeryToken]
        public IActionResult Unlike(IFormCollection collection)
        {
            return WithAuth(user =>
            {
                String? cheepId = collection["cheepId"];
                if (String.IsNullOrEmpty(cheepId))
                {
                    return BadRequest("Invalid input");
                }
                Guid cId = Guid.Parse(cheepId);
                LikeRepository.UnlikeCheep(user.Id, cId);
                
                return Redirect(GetPathUrl());
            });
        }
        

        // POST: Cheep/Create
        [HttpPost]
        [Route("Cheep/Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            return WithAuth(_ =>
            {
                string? cheepText = collection["cheepText"];

                if (String.IsNullOrEmpty(cheepText))
                {
                    return BadRequest("Invalid input");
                }

                if (cheepText.Length > 160)
                {
                    return BadRequest("Invalid input - too long");
                }

                CheepRepository.AddCheep(new AddCheepDto
                {
                    AuthorEmail = GetUserEmail(),
                    AuthorName = GetUserFullName(),
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
        public IActionResult Delete(IFormCollection collection)
        {
            return WithAuth(user =>
            {
                string? cheepId = collection["cheepId"];
                
                if (String.IsNullOrEmpty(cheepId))
                {
                    return BadRequest("Invalid Cheep Id");
                }
                
                Guid id = Guid.Parse(cheepId);
                if (!CheepRepository.DeleteCheep(id, user.Id)) return NotFound("ERROR: Cheep was not found");
                
                return Redirect(GetPathUrl());
            });
        }
        
        //Post Cheep/Follow
        [HttpPost]
        [Route("Cheep/Follow")]
        [ValidateAntiForgeryToken]
        public IActionResult Follow(IFormCollection collection)
        {
            return WithAuth(_ =>
            {
                String authorToBeFollowed = collection["CheepAuthorEmail"].ToString();//The new account to follow
                
                AuthorRepository.AddFollow(User.GetUserEmail(), authorToBeFollowed);
                
                return Redirect(GetPathUrl());//Redirect to same page
            });
        }
        
        //Post Cheep/Unfollow
        [HttpPost]
        [Route("Cheep/Unfollow")]
        [ValidateAntiForgeryToken]
        public IActionResult Unfollow(IFormCollection collection)
        {
            return WithAuth(_ =>
            {
                String authorToBeUnfollowed = collection["CheepAuthorEmail"].ToString();//The new account to follow
                AuthorRepository.RemoveFollow(User.GetUserEmail(), authorToBeUnfollowed);
                return Redirect(GetPathUrl());//Redirect to same page
            });
        }
    }
}