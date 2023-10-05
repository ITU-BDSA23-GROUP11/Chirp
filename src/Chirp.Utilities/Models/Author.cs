namespace Chirp.Utilities.Models;

public class Author
{
    private string Id { get; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public ICollection<Cheep> Cheeps { get; set; } = new HashSet<Cheep>(); // Consider if necessary
}