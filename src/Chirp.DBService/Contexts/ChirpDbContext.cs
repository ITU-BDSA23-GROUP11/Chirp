using Chirp.DBService.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.DBService.Contexts;

// Source: https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli

public class ChirpDbContext : DbContext
{
    public virtual DbSet<Cheep> Cheeps { get; set; } = null!;
    public virtual DbSet<Author> Authors { get; set; } = null!;
    
    public ChirpDbContext()
    { }
    
    public ChirpDbContext(DbContextOptions<ChirpDbContext> options)
        : base(options)
    { }
    
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
}