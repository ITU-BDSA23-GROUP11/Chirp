using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.DBService.Models;

public class Cheep
{
    public int CheepId { get; set; }
    private Author _author = null!;
    public Author Author
    {
        get => _author;
        init
        {
            _author = value;
            value.Cheeps.Add(this);
        }
    }
    public string Text { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}