using Chirp.Core.Repositories;
using Chirp.Core.Dto;
using Chirp.WebService.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.WebService.Controllers
{
    public class CheepController : BaseController
    {
        public CheepController(IAuthorRepository authorRepository, ICheepRepository cheepRepository) : base(authorRepository, cheepRepository)
        {
        }

        // POST: Cheep/Create
        [HttpPost]
        [Route("Cheep/Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
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
            catch
            {
                return BadRequest("Unknown Error Occurred");
            }
        }
        
        // POST: Cheep/Delete
        [HttpPost]
        [Route("Cheep/Delete")]
        [ValidateAntiForgeryToken]
        //public IActionResult Delete(Guid id)
        public IActionResult Delete(IFormCollection collection)
        {
            try
            {
                if (User.Identity != null)
                {
                    String id = collection["cheepId"].ToString();
                    bool isDeleted = CheepRepository.DeleteCheep(id, GetUserFullName());
                    
                    if (!isDeleted)
                    {
                        return NotFound("ERROR: Cheep was not found");
                    }
                    return Redirect(Request.GetPathUrl());
                } 
                return Unauthorized();
            }
            catch
            {
                return BadRequest("An unknown error occured");
            }
       
        }
        
        //Post Cheep/Follow
        [HttpPost]
        [Route("Cheep/Follow")]
        [ValidateAntiForgeryToken]
        public IActionResult Follow(IFormCollection collection)
        {
            return WithAuth((_ =>
            {
                String authorToBeFollowed = collection["CheepAuthorEmail"].ToString();//The new account to follow
                
                CheepRepository.AddFollow(User.GetUserEmail(), authorToBeFollowed);
                
                return Redirect(Request.GetPathUrl());//Redirect to same page
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
                CheepRepository.RemoveFollow(User.GetUserEmail(), authorToBeUnfollowed);
                return Redirect(Request.GetPathUrl());//Redirect to same page
            });
        }
    }
}