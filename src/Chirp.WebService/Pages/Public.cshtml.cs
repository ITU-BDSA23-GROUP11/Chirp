using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Chirp.WebService.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.WebService.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;

    public int PageNumber { get; set; }

    public List<string> Follows { get; set; } = new ();
    public List<CheepDto> Cheeps { get; set; } = new ();

    public int AmountOfPages { get; set; }

    public PublicModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
    }

    public ActionResult OnGet()
    {
        //Calculate the amount of pages needed
        AmountOfPages = (int)Math.Ceiling((double)_cheepRepository.GetCheepCount() / 32);
        
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
        
        Cheeps = _cheepRepository.GetCheepsForPage(PageNumber);
        
        //Set the follows
        Follows = _authorRepository.GetFollowsForAuthor(User.GetUserEmail());
        
        return Page();
    }
}