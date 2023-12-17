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
        MaxLength(160, ErrorMessage = "Cheeps must contain less than 160 characters") //Defined minimum length is not required
    ]
    public string Text { get; set; } = "";
    public ICollection<Comment> Comments { get; } = new List<Comment>();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}