using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Chirp.WebService.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.WebService.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _service;
    
    public int PageNumber { get; set; }
    public List<CheepDto> Cheeps { get; set; } = new ();
    
    public int AmountOfPages { get; set; }

    public UserTimelineModel(ICheepRepository service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author)
    {
        //Calculate the amount of pages needed
        AmountOfPages = (int)Math.Ceiling((double)_service.GetAuthorCheepCount(author) / 32);
        
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
        
        Cheeps = _service.GetAuthorCheepsForPage(author, PageNumber);
        
        return Page();   
    }

    public ActionResult OnPost(string cheepText)
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            _service.AddCheep(new AddCheepDto
            {
                AuthorEmail = User.GetUserEmail(),
                AuthorName = User.GetUserFullName(),
                Text = cheepText
            });
        }
        
        return OnGet(User.GetUserFullName());
    }
}