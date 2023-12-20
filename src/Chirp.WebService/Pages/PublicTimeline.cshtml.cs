using Chirp.Core.Dto;
using Chirp.Core.Extensions;
using Chirp.Core.Repositories;
using Chirp.WebService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.WebService.Pages;

public class PublicTimelineModel: PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ILikeRepository _likeRepository;
    
    public List<CheepPartialModel> Cheeps { get; set; } = new ();
    public FooterPartialModel FooterPartialModel { get; set; }
    
    public PublicTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, ILikeRepository likeRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _likeRepository = likeRepository;
    }

    public async Task<IActionResult> OnGet()
    {
        var cheepCount = await _cheepRepository.GetCheepCount();
        var amountOfPages = (int)Math.Ceiling((double)cheepCount / 32);
        int pageNumber = 1;
        
        if (Request.Query.ContainsKey("page") && int.TryParse(Request.Query["page"], out int pageParameter))
        {
            //If parameter is too large -> set to max
            pageNumber = (pageParameter > amountOfPages) ? amountOfPages : pageParameter;
        }
        
        var cheepDtos = await _cheepRepository.GetCheepsForPage(pageNumber);
        
        var cheepPartialModels = new List<CheepPartialModel>();

        List<string>? follows = null;
        List<LikeDto>? likes = null;
        
        //If the user is not null -> update the list of follows and likes
        var user = User.GetUser();
        if (user is not null)
        {
            follows = await _authorRepository.GetFollowsForAuthor(user.GetUserNonNull().Id);
            likes = await _likeRepository.GetLikesByAuthorId(user.GetUserNonNull().Id);
        }
        
        //Generate a cheep model for each CheepDto on page
        foreach (CheepDto cheepDto in cheepDtos)
        {
            cheepPartialModels.Add(CheepPartialModel.BuildCheepPartialModel(cheepDto, likes, follows));
        }

        Cheeps = cheepPartialModels;

        FooterPartialModel = FooterPartialModel.BuildFooterPartialModel(pageNumber, amountOfPages);
        
        return Page();
    }
}