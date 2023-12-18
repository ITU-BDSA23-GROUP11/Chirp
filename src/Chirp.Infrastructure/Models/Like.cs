using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure.Models;

public class Like
{
    [Key]
    public Guid LikeId { get; set; }
    private Cheep _cheep = null!;
    public Cheep Cheep
    {
        get => _cheep;
        set
        {
            value.Likes.Add(this);
            _cheep = value;
        }
    }
    
    private Author _likedByAuthor = null!;
    public Author LikedByAuthor
    {
        get => _likedByAuthor;
        set
        {
            value.Likes.Add(this);
            _likedByAuthor = value;
        }
    }
}