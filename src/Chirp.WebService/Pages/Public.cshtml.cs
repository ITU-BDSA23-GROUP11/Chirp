using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Models;
using Chirp.WebService.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.WebService.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ILikeRepository _likeRepository;

    public int PageNumber { get; set; }
    public List<string> Follows { get; set; } = new ();
    public List<CheepDto> Cheeps { get; set; } = new ();
    public List<Like> Likes { get; set; } = new ();
    public int AmountOfPages { get; set; }

    public PublicModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, ILikeRepository likeRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _likeRepository = likeRepository;
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
        User.GetUser().RunIfNotNull(user =>
        {
            Follows = _authorRepository.GetFollowsForAuthor(user.Id);
        });
        
        return Page();
    }

    public bool CheepIsLiked(Guid cheepId)
    {
        var authorId = User.GetUser()?.Id ?? Guid.Empty;
        if (authorId.ToString().Equals(Guid.Empty.ToString())) return false;
        return _likeRepository.IsLiked(authorId, cheepId);   
    
    }
}