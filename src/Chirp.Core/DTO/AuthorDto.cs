using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.DTO;

// Principal (parent)
public class AuthorDto
{
    [Key]
    public Guid AuthorId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public ICollection<CheepDto> Cheeps { get; set; } = new List<CheepDto>();
}