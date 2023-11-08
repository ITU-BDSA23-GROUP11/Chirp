using Chirp.Core.Repositories;
using Chirp.Core.Dto;
using Chirp.WebService.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.WebService.Controllers
{
    public class CheepController : Controller
    {
        private readonly ICheepRepository _service;

        public CheepController(ICheepRepository service)
        {
            _service = service;
        }

        // POST: Cheep/Create
        [HttpPost]
        [Route("Cheep/Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                string? cheepText = collection["cheepText"];

                if (cheepText == null)
                {
                    return BadRequest("Invalid input");
                }
                
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    _service.AddCheep(new AddCheepDto
                    {
                        AuthorEmail = User.GetUserEmail(),
                        AuthorName = User.GetUserFullName(),
                        Text = cheepText
                    });
                }
                else
                {
                    return Unauthorized();
                }

                return Redirect(Request.GetPathUrl());
            }
            catch
            {
                return BadRequest("Unknown Error Occurred");
            }
        }
        // POST: Cheep/Delete
        [HttpDelete]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Guid id)
        {
            try
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    bool isDeleted = _service.DeleteCheep(id, User.Identity.Name);
                    
                    if (!isDeleted)
                    {
                        return NotFound("ERROR: Cheep was not found");
                    }
                }
                
                {
                    return Unauthorized();
                }
            }
            catch
            {
                return BadRequest("An unknown error occured");
            }
       
        }
        
        
    }
}