using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure.Models;

public class Cheep
{
    [Key]
    public Guid CheepId { get; set; }
    
    private Author _author = null!;
    
    public Author Author
    {
        get => _author;
        set
        {
            value.Cheeps.Add(this);
            _author = value;
        }
    }
    
    [
        Required,
        MaxLength(160, ErrorMessage = "Cheeps must contain less than 160 characters")
    ]
    public string Text { get; set; } = "";
    public ICollection<Like> Likes { get; } = new List<Like>();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public ICollection<Comment> Comments { get; } = new List<Comment>();
}