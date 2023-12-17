using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure.Models;

public class Comment
{
    [Key]
    public Guid CommentId { get; set; }
    
    private Author _author = null!;
    
    public Author Author
    {
        get => _author;
        set
        {
            _author = value;
        }
    }
    
    private Cheep _cheep = null!;
    
    public Cheep Cheep
    {
        get => _cheep;
        set
        {
            _cheep = value;
        }
    }
    
    [
        Required,
        MaxLength(160, ErrorMessage = "Comment must contain less than 160 characters")
    ]
    public string Text { get; set; } = "";
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}