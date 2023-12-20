namespace Chirp.WebService.Models;

public struct CommentPartialModel
{
    public Guid CheepId;
    public Guid AuthorId;
    public Guid CheepAuthorId;
    public Guid CommentId;
    public string AuthorAvatarUrl;
    public string AuthorName;
    public string AuthorUsername;
    public DateTime Timestamp;
    public string Text;
}