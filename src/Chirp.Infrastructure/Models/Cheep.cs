using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Chirp.Infrastructure.Models;

public class Cheep
{
    [Key]
    public Guid CheepId { get; set; }
    private Author _author = null!;
    public Author Author
    {
        get => _author;
        set
        {
            value.Cheeps.Add(this);
            _author = value;
        }
    }
    [Required]
    [MinLength(1, ErrorMessage = "Cheep must contain more than 1 character")]
    [MaxLength(160, ErrorMessage = "Cheeps must contain less than 160 characters")]
    public string Text { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}