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
    
    public async Task<IActionResult> OnGet()
    {
        var user = User.GetUser();
        if (user is not null)
        {
            await _authorRepository.DeleteAuthor(user.GetUserNonNull().Id);
            Response.Cookies.Delete(".AspNetCore.Cookies");
        }
        
        return Redirect("/");
    }
}