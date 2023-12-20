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
            cheepPartialModels.Add(new CheepPartialModel
            {
                CheepId = cheepDto.CheepId,
                AuthorId = cheepDto.AuthorId,
                AuthorAvatarUrl = cheepDto.AuthorAvatarUrl,
                AuthorName = cheepDto.AuthorName ?? cheepDto.AuthorUsername,
                AuthorUsername = cheepDto.AuthorUsername,
                Timestamp = cheepDto.Timestamp,
                Text = cheepDto.Text,
                LikesAmount = cheepDto.LikeCount,
                IsLikedByUser = (likes is null
                    ? null
                    : likes.Any(l => l.CheepId.ToString().Equals(cheepDto.CheepId.ToString()))),
                IsFollowedByUser = (follows is null ? null : !follows.Contains(cheepDto.AuthorUsername)),
                CheepComments = cheepDto.CommentDtos.Select(c =>
                    new CommentPartialModel
                    {
                        AuthorAvatarUrl = c.AuthorAvatarUrl,
                        AuthorId = c.AuthorId,
                        CheepAuthorId = c.CheepAuthorId,
                        CommentId = c.CommentId,
                        AuthorUsername = c.AuthorUsername,
                        AuthorName = c.AuthorName,
                        Timestamp = c.Timestamp,
                        Text = c.Text,
                        CheepId = c.CheepId
                    }).ToList()
            });
        }

        Cheeps = cheepPartialModels;

        FooterPartialModel = new FooterPartialModel
        {
            FirstLink = pageNumber > 2 ? "/?page=1" : null,
            
            PreviousLink = pageNumber > 1 ? $"/?page={pageNumber-1}" : null,
            PreviousPage = pageNumber > 1 ? pageNumber-1 : null,
            
            CurrentPage = pageNumber,
            
            NextLink = pageNumber < amountOfPages ? $"/?page={pageNumber+1}" : null,
            NextPage = pageNumber < amountOfPages ? pageNumber+1 : null,
            
            LastLink = pageNumber < amountOfPages-1 ? $"/?page={amountOfPages}" : null,
            LastPage = pageNumber < amountOfPages-1 ? amountOfPages : null,
        };
        
        return Page();
    }
}