namespace Chirp.Core.DTO;

public class CheepDto
{
    public string AuthorName { get; set; } = "";
    public string Text { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}