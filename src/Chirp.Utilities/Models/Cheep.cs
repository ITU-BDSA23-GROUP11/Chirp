namespace Chirp.Utilities.Models;

public class Cheep
{
    private string Id { get; } = Guid.NewGuid().ToString();
    public required Author Author { get; set; }
    public string Message { get; set; } = "";
    private DateTime Timestamp { get; } = DateTime.UtcNow;
}