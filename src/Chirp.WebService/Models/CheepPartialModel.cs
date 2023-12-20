using Chirp.Core.Dto;

namespace Chirp.WebService.Models;
public struct CheepPartialModel
{
    public Guid CheepId;
    public Guid AuthorId;
    public string AuthorAvatarUrl;
    public string AuthorName;
    public string AuthorUsername;
    public DateTime Timestamp;
    public string Text;
    public int LikesAmount;
    public bool? IsFollowedByUser;
    public bool? IsLikedByUser;
    public List<CommentPartialModel> CheepComments;
    
    public static CheepPartialModel BuildCheepPartialModel(
        CheepDto cheepDto,
        List<LikeDto>? likes,
        List<string>? follows
    )
    {
        return new CheepPartialModel
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
        };
    }
}