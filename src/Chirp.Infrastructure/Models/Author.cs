using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure.Models;

// Principal (parent)
public class Author
{
    [Key]
    public Guid AuthorId { get; set; }

    [
        MaxLength(50, ErrorMessage = "Username must contain less than 50 characters"),
        MinLength(5, ErrorMessage = "Username must contain more than 5 characters")
    ]
    public string? Name { get; set; }
    
    [Required]
    public required string Username { get; set; }

    [Required] public required string AvatarUrl { get; set; }
    
    public ICollection<Cheep> Cheeps { get; } = new List<Cheep>();

    public List<Author> Follows { get; set; } = new ();
    public List<Author> FollowedBy { get; set; } = new ();
    public List<Like> Likes { get; set; } = new ();
}