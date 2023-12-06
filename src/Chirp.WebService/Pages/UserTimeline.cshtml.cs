using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Chirp.WebService.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.WebService.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ILikeRepository _likeRepository;
    
    public int PageNumber { get; set; }
    public AuthorDto? Author { get; set; }
    public List<CheepDto> Cheeps { get; set; } = new ();
    public List<string> Follows { get; set; } = new ();

    public int AmountOfPages { get; set; }

    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, ILikeRepository likeRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _likeRepository = likeRepository;
    }

    public ActionResult OnGet(string author)
    {
        Author = _authorRepository.GetAuthorFromUsername(author);
        
        User.GetUser().RunIfNotNull(user =>
        {
            Follows = _authorRepository.GetFollowsForAuthor(user.Id);
        });

        User.GetUser().RunIfNotNull(user =>
        {
            AmountOfPages =
                (int)Math.Ceiling((double)_cheepRepository.GetAuthorCheepCount(user.Username, user.Username.Equals(author)) /
                                  32);
        });
        
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

        User.GetUser().RunIfNotNull(user =>
        {
            if (user.Username.Equals(author))
            {
                Cheeps = _cheepRepository.GetAuthorCheepsForPageAsOwner(user.Id, PageNumber);
            }
            else
            {
                Cheeps = _cheepRepository.GetAuthorCheepsForPage(user.Username, PageNumber);
            }
        });
        
        return Page();   
    }
    
    public bool CheepIsLiked(Guid cheepId)
    {
        var authorId = User.GetUser()?.Id ?? Guid.Empty;
        if (authorId.ToString().Equals(Guid.Empty.ToString())) return false;
        return _likeRepository.IsLiked(authorId, cheepId);   
    
    }

    public int GetLikeCount(Guid cheepId)
    {
        return _likeRepository.LikeCount(cheepId);
    }
}