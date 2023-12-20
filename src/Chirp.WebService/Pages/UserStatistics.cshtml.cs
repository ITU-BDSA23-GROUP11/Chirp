using Chirp.Core.Dto;
using Chirp.Core.Extensions;
using Chirp.Core.Repositories;
using Chirp.WebService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.WebService.Pages;

public class UserStatisticsModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ILikeRepository _likeRepository;

    public List<string> Following { get; set; } = new ();

    public List<LikeDto> Likes { get; set; } = new ();

    public List<CheepPartialModel> LikedCheeps { get; set; } = new ();

    public UserStatisticsModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, ILikeRepository likeRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _likeRepository = likeRepository;
    }
    
    public async Task<IActionResult> OnGet()
    {
        var user = User.GetUser();

        if (user is not null)
        {
            Following = await _authorRepository.GetFollowsForAuthor(user.GetUserNonNull().Id);
            Likes = await _likeRepository.GetLikesByAuthorId(user.GetUserNonNull().Id);
        }

        HashSet<Guid> likeIds = new HashSet<Guid>();
        foreach(LikeDto like in Likes)
        {
            likeIds.Add(like.CheepId);
        }
        
        
        var follows = await _authorRepository.GetFollowsForAuthor(user.GetUserNonNull().Id);
        var likes = await _likeRepository.GetLikesByAuthorId(user.GetUserNonNull().Id);

        LikedCheeps = (await _cheepRepository.GetCheepsFromIds(likeIds))
            .Select(cheepDto =>
                new CheepPartialModel
                {
                    CheepId = cheepDto.CheepId,
                    AuthorId = cheepDto.AuthorId,
                    AuthorAvatarUrl = cheepDto.AuthorAvatarUrl,
                    AuthorName = cheepDto.AuthorName ?? cheepDto.AuthorUsername,
                    AuthorUsername = cheepDto.AuthorUsername,
                    Timestamp = cheepDto.Timestamp,
                    Text = cheepDto.Text,
                    IsLikedByUser = likes.Any(l => l.CheepId.ToString().Equals(cheepDto.CheepId.ToString())),
                    LikesAmount = cheepDto.LikeCount,
                    IsFollowedByUser = !follows.Contains(cheepDto.AuthorUsername),
                    CheepComments = cheepDto.CommentDtos.Select(c => new CommentPartialModel
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
                }
            ).ToList();
        
        return Page();
    }
}