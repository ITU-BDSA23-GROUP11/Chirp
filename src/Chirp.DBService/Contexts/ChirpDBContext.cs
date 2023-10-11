using System.Net.Mime;
using Chirp.DBService.Models;
using Chirp.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Chirp.DBService;

// Source: https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli

public class ChirpDBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cheep>()
            .HasOne(c => c.Author)
            .WithMany(a => a.Cheeps)
            .HasPrincipalKey(a => a.AuthorId)
            .HasForeignKey(c => c.CheepId)
            .IsRequired();
    }

    private string DbPath = Path.Combine(MiscUtilities.TryGetSolutionDirectoryInfo().FullName, "data", "chirp_db.db");
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options
            .UseSqlite($"Data Source={DbPath}")
            .EnableSensitiveDataLogging();
    }
}