namespace Chirp.DBService.Models;

public class Author
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public required string Name { get; init; }
    public ICollection<Cheep> Cheeps { get; } = new List<Cheep>();
}