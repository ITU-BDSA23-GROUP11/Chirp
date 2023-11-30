using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure.Models;

public class Like
{
    [Key]
    public Guid LikedByAuthorId { get; set; }
    public Guid CheepId { get; set; }
}