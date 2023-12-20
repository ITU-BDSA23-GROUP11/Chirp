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
    public required string Name { get; set; }
    
    [
        MaxLength(40, ErrorMessage = "Username must contain less than 40 characters"),
        MinLength(6, ErrorMessage = "Username must contain more than 6 characters")
    ]
    public required string Username { get; set; }

    [
        MaxLength(300, ErrorMessage = "URL must contain less than 300 characters"), 
        MinLength(5, ErrorMessage = "URL must contain more than 5 characters")
    ]
    public required string AvatarUrl { get; set; }
    public ICollection<Cheep> Cheeps { get; } = new List<Cheep>();
    public ICollection<Comment> Comments { get; } = new List<Comment>();
    public ICollection<Author> Follows { get; } = new List<Author>();
    public ICollection<Author> FollowedBy { get; } = new List<Author>();
    public ICollection<Like> Likes { get; } = new List<Like>();
}