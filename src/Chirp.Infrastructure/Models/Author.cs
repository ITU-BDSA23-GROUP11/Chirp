using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure.Models;

// Principal (parent)
public class Author
{
    [Key]
    public Guid AuthorId { get; set; }
    public required string Name { get; set; }
    [Required]
    [MaxLength(75)]
    [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$", 
        ErrorMessage = "Invalid email format.")] 
    public required string Email { get; set; }
    public ICollection<Cheep> Cheeps { get; } = new List<Cheep>();
}