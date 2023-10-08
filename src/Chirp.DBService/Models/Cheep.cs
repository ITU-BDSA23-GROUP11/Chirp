using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.DBService.Models;

public class Cheep
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public string AuthorId = null!;
    [NotMapped]
    private readonly Author _author = null!;
    public required Author Author
    {
        get => _author;
        init
        {
            _author = value;
            AuthorId = value.Id;
            value.Cheeps.Add(this);
        }
    }
    public string Message { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}