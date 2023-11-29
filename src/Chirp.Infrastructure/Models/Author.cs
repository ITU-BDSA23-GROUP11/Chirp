using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure.Models;

// Principal (parent)
public class Author
{
    [Key]
    public Guid AuthorId { get; set; }
    
    [
        Required,
        MaxLength(50, ErrorMessage = "Username must contain less than 50 characters"),
        MinLength(5, ErrorMessage = "Username must contain more than 5 characters")
    ]
    public required string Name { get; set; }
    
    [
        Required,
        RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$", ErrorMessage = "Invalid email format."),
        MaxLength(100, ErrorMessage = "Email must be less than 100 characters")
    ]
    public required string Email { get; set; }
    
    public ICollection<Cheep> Cheeps { get; } = new List<Cheep>();

    public List<Author> Follows { get; set; } = new ();
    public List<Author> FollowedBy { get; set; } = new ();
}