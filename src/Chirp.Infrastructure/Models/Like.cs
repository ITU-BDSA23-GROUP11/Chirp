using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure.Models;

public class Like
{
    [Key]
    public Guid LikeId { get; set; }
    public Cheep Cheep { get; set; } = null!;
    public Author LikedByAuthor { get; set; } = null!;

}