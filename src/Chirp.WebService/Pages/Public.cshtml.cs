using Chirp.Core.DTO;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.WebService.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _service;

    public int PageNumber { get; set; }

    public List<CheepDto> Cheeps { get; set; } = new List<CheepDto>();

    public int AmountOfPages { get; set; }

    public PublicModel(ICheepRepository service)
    {
        _service = service;
    }

    public ActionResult OnGet()
    {
        //Calculate the amount of pages needed
        AmountOfPages = (int)Math.Ceiling((double)_service.GetCheepCount() / 32);
        
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
        
        Cheeps = _service.GetCheepsForPage(PageNumber);
        
        return Page();
    }
}