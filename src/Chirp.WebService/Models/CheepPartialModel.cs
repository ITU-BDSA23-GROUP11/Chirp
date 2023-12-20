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
}