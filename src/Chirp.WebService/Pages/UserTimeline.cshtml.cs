using Chirp.DBService.Models;
using Chirp.DBService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.WebService.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _service;
    
    public int pageNumber { get; set; }
    public List<Cheep> Cheeps { get; set; }
    
    public int AmountOfPages { get; set; }

    public UserTimelineModel(ICheepRepository service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author)
    {
        //Determine pageNumber
        if (Request.Query.ContainsKey("page") && int.TryParse(Request.Query["page"], out int _pageNumber))
        {
            //If parameter is too large -> set to max
            if (_pageNumber > AmountOfPages) pageNumber = AmountOfPages;
            else pageNumber = _pageNumber;
        }
        else
        {
            //Fallback page 0
            pageNumber = 0;
        }
        
        Cheeps = _service.GetCheepsFromAuthorNameForPage(author, pageNumber);
            
        //Calculate the amount of pages needed
        AmountOfPages = (int)Math.Ceiling((double)_service.GetCheepsFromAuthorNameWithAuthors(author).Count() / 32);
        return Page();
    }
}