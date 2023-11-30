using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.WebService.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    
    public int PageNumber { get; set; }
    public List<CheepDto> Cheeps { get; set; } = new ();
    public List<string> Follows { get; set; } = new ();

    public int AmountOfPages { get; set; }

    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
    }

    public ActionResult OnGet(string author)
    {
        //Set the follows
        if (!User.GetUserEmail().Equals(("No Email")))
        {
            Follows = _authorRepository.GetFollowsForAuthor(User.GetUserEmail());
        }
        
        //Calculate the total amount of pages needed for pagination
        if (User.GetUserFullName().Equals(author))
        {
            //The user is the owner -> include follows
            int allCheepsCount = 0;
            allCheepsCount += _cheepRepository.GetAuthorCheepCount(author);
            foreach (string f in Follows) allCheepsCount += _cheepRepository.GetAuthorCheepCount(_authorRepository.GetAuthorNameByEmail(f));
            AmountOfPages = (int)Math.Ceiling((double)allCheepsCount / 32);
        }
        else
        {
            //The user is a visitor
            AmountOfPages = (int)Math.Ceiling((double)_cheepRepository.GetAuthorCheepCount(author) / 32);
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
            Cheeps = _cheepRepository.GetAuthorCheepsForPageAsOwner(author, PageNumber);
        }
        else
        {
            Cheeps = _cheepRepository.GetAuthorCheepsForPage(author, PageNumber);
        }
        
        return Page();   
    }
}