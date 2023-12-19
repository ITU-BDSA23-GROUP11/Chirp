using Chirp.Core.Dto;
using Chirp.Core.Extensions;
using Chirp.Core.Repositories;
using Chirp.WebService.Extensions;
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

    public ActionResult OnGet()
    {
        var amountOfPages = (int)Math.Ceiling((double)_cheepRepository.GetCheepCount() / 32);
        int pageNumber = 1;
        
        if (Request.Query.ContainsKey("page") && int.TryParse(Request.Query["page"], out int pageParameter))
        {
            //If parameter is too large -> set to max
            pageNumber = (pageParameter > amountOfPages) ? amountOfPages : pageParameter;
        }
        
        var cheepDtos = _cheepRepository.GetCheepsForPage(pageNumber);
        
        var cheepPartialModels = new List<CheepPartialModel>();
        
        //Set the follows
        User.GetUser().RunIfNotNull(user =>
        {
            var follows = _authorRepository.GetFollowsForAuthor(user.Id);
            var likes = _likeRepository.GetLikesByAuthorId(user.Id);
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
                    isLikedByUser = likes.Any(l => l.CheepId.ToString().Equals(cheepDto.CheepId.ToString())),
                    likesAmount = _likeRepository.LikeCount(cheepDto.CheepId),
                    isFollowedByUser = !follows.Contains(cheepDto.AuthorUsername),
                    CheepComments = cheepDto.CommentDtos.Select<CommentDto, CommentPartialModel>(c => new CommentPartialModel
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
        }, () =>
        {
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
                    likesAmount = _likeRepository.LikeCount(cheepDto.CheepId),
                    isLikedByUser = null,
                    isFollowedByUser = null,
                    CheepComments = cheepDto.CommentDtos.Select<CommentDto, CommentPartialModel>(c => new CommentPartialModel
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
        });

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