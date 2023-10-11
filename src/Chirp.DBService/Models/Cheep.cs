using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.DBService.Models;

public class Cheep
{
    public Guid CheepId { get; set; }
    private Author _author = null!;
    public Author Author
    {
        get => _author;
        set
        {
            if (!value.Cheeps.Any(c => c.CheepId == CheepId))
            {
                value.Cheeps.Add(this);
            }
            _author = value;
        }
    }
    public string Text { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}