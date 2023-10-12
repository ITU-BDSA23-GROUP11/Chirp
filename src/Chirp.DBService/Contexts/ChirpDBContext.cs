using Chirp.DBService.Models;
using Chirp.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chirp.DBService;

// Source: https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli

public class ChirpDBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    
    private string DbPath { get; } = Path.Join(Path.GetTempPath(), "chirp_db.db");
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cheep>()
            .HasOne<Author>(c => c.Author)
            .WithMany(a => a.Cheeps)
            .HasForeignKey("AuthorId")
            .IsRequired();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options
            .UseSqlite($"Data Source={DbPath}");
        Console.WriteLine("ChirpDBContext database initialised at:\n"+DbPath);
    }
}