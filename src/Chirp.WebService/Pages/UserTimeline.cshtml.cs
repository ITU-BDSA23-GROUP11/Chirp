using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Chirp.WebService.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

namespace Chirp.WebService.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _service;
    
    public int PageNumber { get; set; }
    public List<CheepDto> Cheeps { get; set; } = new ();
    public List<string> Follows { get; set; } = new List<string>();

    public int AmountOfPages { get; set; }

    public UserTimelineModel(ICheepRepository service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author)
    {
        //Set the follows
        if (!User.GetUserEmail().Equals(("No Email")))
        {
            Follows = _service.GetFollowsForAuthor(User.GetUserEmail());
        }
        
        //Calculate the total amount of pages needed for pagination
        if (User.GetUserFullName().Equals(author))
        {
            //The user is the owner -> include follows
            int allCheepsCount = 0;
            allCheepsCount += _service.GetAuthorCheepCount(author);
            foreach (string f in Follows) allCheepsCount += _service.GetAuthorCheepCount(_service.GetAuthorNameByEmail(f));
            AmountOfPages = (int)Math.Ceiling((double)allCheepsCount / 32);
        }
        else
        {
            //The user is a visitor
            AmountOfPages = (int)Math.Ceiling((double)_service.GetAuthorCheepCount(author) / 32);
        }

        //Determine pageNumber
        if (Request.Query.ContainsKey("page") && int.TryParse(Request.Query["page"], out int pageParameter))
        { 
            //If parameter is too large -> set to max
            if (pageParameter > AmountOfPages) PageNumber = AmountOfPages;
            else PageNumber = pageParameter;
        }
        else
        {
            //Fallback page 0
            PageNumber = 0;
        }

        if (User.GetUserFullName().Equals(author))
        {
            Cheeps = _service.GetAuthorCheepsForPageAsOwner(author, PageNumber);
        }
        else
        {
            Cheeps = _service.GetAuthorCheepsForPage(author, PageNumber);
        }
        
        return Page();   
    }
}