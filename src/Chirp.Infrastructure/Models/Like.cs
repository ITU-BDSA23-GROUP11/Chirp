using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure.Models;

public class Like
{
    [Key]
    public Guid CheepId { get; set; }
    public Guid LikedByAuthorId { get; set; }
   
}