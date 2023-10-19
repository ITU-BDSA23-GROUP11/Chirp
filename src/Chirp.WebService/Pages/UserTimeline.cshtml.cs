using Chirp.DBService.Models;
using Chirp.DBService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.WebService.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _service;
    
    public int PageNumber { get; set; }
    public List<Cheep>? Cheeps { get; set; } = new List<Cheep>();
    
    public int AmountOfPages { get; set; }

    public UserTimelineModel(ICheepRepository service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author)
    {
        //Calculate the amount of pages needed
        AmountOfPages = (int)Math.Ceiling((double)_service.GetCheepsFromAuthorNameWithAuthors(author).Count() / 32);
        
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
        
        Cheeps = _service.GetCheepsFromAuthorNameForPage(author, PageNumber);
        
        return Page();   
    }
}