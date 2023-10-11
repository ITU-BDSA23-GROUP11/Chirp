using Chirp.DBService.Models;
using Chirp.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Chirp.DBService;

// Source: https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli

public class ChirpDBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    private static bool IsTest { get; set; }

    public ChirpDBContext(bool isTest = false)
    {
        IsTest = isTest;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cheep>()
            .HasOne<Author>(c => c.Author)
            .WithMany(a => a.Cheeps)
            .HasForeignKey("AuthorId")
            .IsRequired();
    }

    private string DbPath
    {
        get
        {
            string path = Path.Combine(MiscUtilities.TryGetSolutionDirectoryInfo().FullName, "data" );
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return Path.Combine(path, IsTest ? "chirp_test_db.db" : "chirp_db.db");
        }
    }

    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options
            .UseSqlite($"Data Source={DbPath}");

    }
}