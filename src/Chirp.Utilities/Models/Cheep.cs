namespace Chirp.Utilities.Models;

public class Cheep
{
    public string Id { get; } = Guid.NewGuid().ToString();
    
    // 1 Cheep to 1 Author relation
    public string AuthorId { get; set; }
    public required Author Author { get; set; }
    
    public string Message { get; set; } = "";
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}