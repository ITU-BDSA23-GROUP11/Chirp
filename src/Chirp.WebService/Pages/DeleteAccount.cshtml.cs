using Chirp.Core.Extensions;
using Chirp.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.WebService.Pages;

public class DeleteAccountModel: PageModel
{
    private readonly IAuthorRepository _authorRepository;

    public DeleteAccountModel(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }
    
    public ActionResult OnGet()
    {
        User.GetUser().RunIfNotNull(user =>
        {
            // Delete user
            _authorRepository.DeleteAuthor(user.Id);
            
            // Sign user out
            Response.Cookies.Delete(".AspNetCore.Cookies");
        });
        
        return Redirect("/");
    }
}