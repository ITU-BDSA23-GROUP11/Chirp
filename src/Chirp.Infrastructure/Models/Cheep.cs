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
    public string Text { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}