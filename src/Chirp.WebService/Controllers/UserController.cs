using Chirp.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.WebService.Controllers;

public class UserController : BaseController
{
    
    public UserController(IAuthorRepository authorRepository, ICheepRepository cheepRepository, ILikeRepository likeRepository) : base(authorRepository, cheepRepository, likeRepository)
    {
    }
    
    // POST: User/Delete
    [HttpPost]
    [Route("User/Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteAccount()
    {
        return WithAuth(user =>
        {
            // TODO 1: Delete user occurrences in DB
            var result = AuthorRepository.DeleteAuthor(user.Id);
            if (!result)
            {
                // TODO: Return error occurred response
            }
            
            // TODO 2: Initiate delete user endpoint
            
            // TODO 3: Sign user out
            
            // TODO 4: Redirect to previous page
                
            return Redirect(GetPathUrl());
        });
    }
}