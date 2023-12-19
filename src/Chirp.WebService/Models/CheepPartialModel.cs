using Chirp.Core.Dto;
using Chirp.Infrastructure.Models;

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
    public int likesAmount;
    public bool? isFollowedByUser;
    public bool? isLikedByUser;
    public List<CommentPartialModel> CheepComments;
}