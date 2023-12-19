using Chirp.Core.Dto;
using Chirp.Core.Extensions;
using Chirp.Core.Repositories;
using Chirp.WebService.Extensions;
using Chirp.WebService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.WebService.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ILikeRepository _likeRepository;
    
    public List<CheepPartialModel> Cheeps { get; set; } = new ();
    public FooterPartialModel FooterPartialModel { get; set; }

    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, ILikeRepository likeRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _likeRepository = likeRepository;
    }
    
    public ActionResult OnGet(string author)
    {
        // Get amount of pages
        var amountOfPages = User.GetUser().RunIfNotNull(user =>
        {
            return (int)Math.Ceiling((double)_cheepRepository.GetAuthorCheepCount(author, user.Id) / 32);
        }, (int)Math.Ceiling((double)_cheepRepository.GetAuthorCheepCount(author) / 32));
        
        // Get page number
        int pageNumber = 1;
        if (Request.Query.ContainsKey("page") && int.TryParse(Request.Query["page"], out int pageParameter))
        {
            //If parameter is too large -> set to max
            pageNumber = (pageParameter > amountOfPages) ? amountOfPages : pageParameter;
        }

        // Get cheep dtos
        var cheepDtos = new List<CheepDto>();
        User.GetUser().RunIfNotNull(user =>
        {
            if (user.Username.Equals(author))
            {
                cheepDtos = _cheepRepository.GetAuthorCheepsForPageAsOwner(user.Id, pageNumber);
            }
            else
            {
                cheepDtos = _cheepRepository.GetAuthorCheepsForPage(author, pageNumber);
            }
        }, () =>
        {
            cheepDtos = _cheepRepository.GetAuthorCheepsForPage(author, pageNumber);
        });
        
        // Build models
        var cheepPartialModels = new List<CheepPartialModel>();
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
            FirstLink = pageNumber > 2 ? $"/{author}?page=1" : null,
            
            PreviousLink = pageNumber > 1 ? $"/{author}?page={pageNumber-1}" : null,
            PreviousPage = pageNumber > 1 ? pageNumber-1 : null,
            
            CurrentPage = pageNumber,
            
            NextLink = pageNumber < amountOfPages ? $"/{author}?page={pageNumber+1}" : null,
            NextPage = pageNumber < amountOfPages ? pageNumber+1 : null,
            
            LastLink = pageNumber < amountOfPages-1 ? $"/{author}?page={amountOfPages}" : null,
            LastPage = pageNumber < amountOfPages-1 ? amountOfPages : null,
        };
        
        return Page();
    }
}