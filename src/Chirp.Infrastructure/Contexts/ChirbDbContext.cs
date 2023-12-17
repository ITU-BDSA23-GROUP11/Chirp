using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Contexts;

// Source: https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli

public class ChirpDbContext : DbContext
{
    public virtual DbSet<Cheep> Cheeps { get; set; } = null!;
    public virtual DbSet<Author> Authors { get; set; } = null!;
    public virtual DbSet<Like> Likes { get; set; } = null!;
    public ChirpDbContext() {}

    public ChirpDbContext(DbContextOptions<ChirpDbContext> options)
        : base(options)
    {
       
        Database.Migrate(); // Required for tests
        DbInitializer.SeedDatabase(this);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cheep>()
            .HasOne<Author>(c => c.Author)
            .WithMany(a => a.Cheeps)
            .HasForeignKey("AuthorId")
            .IsRequired();

        modelBuilder.Entity<Author>()
            .HasMany<Author>(a => a.Follows)
            .WithMany(c => c.FollowedBy);
        
        modelBuilder.Entity<Like>().HasKey(x => new { x.LikedByAuthorId, x.CheepId });
    }
}