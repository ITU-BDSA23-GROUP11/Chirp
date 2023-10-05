namespace Chirp.Utilities.Models;

public class Author
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public required string Name { get; set; }
    
    // 1 Author to Many Cheeps relation
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();
}