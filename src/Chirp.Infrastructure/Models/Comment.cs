using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure.Models;
/*Model of a comment that will be shown on each cheep*/
public class Comment
{
    public Guid CommentId { get; set; }
    private Cheep _cheep = null!;
    public Cheep Cheep
    {
        get => _cheep;
        set
        {
            value.Comments.Add(this);
            _cheep = value;
        }
    }
    
    public string? UserName { get; set; }
    private Author _author = null!;
    public Author Author
    {
        get => _author;
        set
        {
            value.Comments.Add(this);
            _author = value;
        }
    }

    [
        Required,
        MaxLength(160, ErrorMessage = "Comment must contain less than 160 characters") //Defined minimum length is not required
    ]
    public required string Text { get; set; } = "";

    public required DateTime Timestamp { get; set; } = DateTime.UtcNow;
}