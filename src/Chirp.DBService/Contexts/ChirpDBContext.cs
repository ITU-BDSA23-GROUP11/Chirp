using Chirp.DBService.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.DBService;

// Source: https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli

public class ChirpDBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    
    private string DbPath { get; }
    
    public ChirpDBContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "chirp_db.db");
        Console.WriteLine("ChirpDBContext database initialised at:\n"+DbPath);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cheep>()
            .HasOne<Author>(c => c.Author)
            .WithMany(a => a.Cheeps)
            .HasForeignKey("AuthorId")
            .IsRequired();
        modelBuilder.Entity<Author>().Property(a => a.Name).HasMaxLength(50).IsRequired(); //Restriction of maximum length
        modelBuilder.Entity<Author>().Property(e => e.Email).HasMaxLength(50).IsRequired(); //Restriction of maximum length
        modelBuilder.Entity<Author>().HasIndex(a => a.Name).IsUnique(); //Restrictions of uniqueness
            
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options
            .UseSqlite($"Data Source={DbPath}");

    }
}