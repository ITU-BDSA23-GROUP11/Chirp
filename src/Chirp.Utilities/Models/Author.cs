namespace Chirp.Utilities.Models;

public class Author
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public required string Name { get; set; }
    public ICollection<Cheep> Cheeps { get; set; } = new HashSet<Cheep>(); // Consider if necessary

    public Author(string name)
    {
        Name = name;
    }
    
    public void AddCheep(Cheep cheep)
    {
        Cheeps.Add(cheep);
    }
}