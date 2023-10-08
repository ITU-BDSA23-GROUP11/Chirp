using Chirp.DBService.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.DBService;

// Source: https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli

public class ChirpDBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>()
            .HasMany<Cheep>(a => a.Cheeps)
            .WithOne(c => c.Author)
            .HasForeignKey(c => c.AuthorId)
            .HasPrincipalKey(a => a.Id);

        modelBuilder.Entity<Cheep>()
            .HasKey(c => c.Id);
    }

    public string DbPath
    {
        get
        {
            return  "../../data/chirp_db.db";
            
            // var folder = Environment.SpecialFolder.LocalApplicationData;
            // var path = Environment.GetFolderPath(folder);
            // return Path.Join(path, "cheeps.db");
        }
    }
    
    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}