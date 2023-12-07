using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Repositories;
using Chirp.WebService.Extensions;
using Chirp.WebService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.WebService.Pages;

public class UserStatisticsModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ILikeRepository _likeRepository;
    
    public List<CheepPartialModel> Cheeps { get; set; } = new ();

    public List<string>? Following { get; set; } = new List<string>();

    public List<LikeDto> Likes { get; set; } = new List<LikeDto>();

    public List<CheepDto> LikesCheeps { get; set; } = new List<CheepDto>();
    public FooterPartialModel FooterPartialModel { get; set; }

    public UserStatisticsModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, ILikeRepository likeRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _likeRepository = likeRepository;
    }
    
    public ActionResult OnGet(string author)
    {
        Following = _authorRepository.GetFollowsForAuthor(author);

        AuthorDto? authorDto = _authorRepository.GetAuthorFromUsername(author);

        Likes = _likeRepository.GetLikesByAuthorId(authorDto.Id);

        HashSet<Guid> likeIds = new HashSet<Guid>();
        
        foreach(LikeDto like in Likes)
        {
            likeIds.Add(like.CheepId);
        }

        //From the likes -> find liked cheeps
        LikesCheeps = _cheepRepository.GetCheepsFromIds(likeIds);
        
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
                    isFollowedByUser = !follows.Contains(cheepDto.AuthorUsername)
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
                    isFollowedByUser = null
                });
            }
        });

        Cheeps = cheepPartialModels;

        FooterPartialModel = new FooterPartialModel
        {
            FirstLink = pageNumber > 2 ? $"/?page=1" : null,
            
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