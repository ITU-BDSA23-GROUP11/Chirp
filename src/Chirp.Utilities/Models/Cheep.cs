namespace Chirp.Utilities.Models;

public class Cheep
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public required Author Author { get; set; }
    public string Message { get; set; } = "";
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}