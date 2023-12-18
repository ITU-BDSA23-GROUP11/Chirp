using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Contexts;

// Source: https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli

public class ChirpDbContext : DbContext
{
    public virtual DbSet<Cheep> Cheeps { get; set; } = null!;
    public virtual DbSet<Author> Authors { get; set; } = null!;
    public virtual DbSet<Like> Likes { get; set; } = null!;
    public virtual DbSet<Comment> Comments { get; set; } = null!;
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
        
        modelBuilder.Entity<Comment>()
            .HasOne<Author>(c => c.CommentAuthor)
            .WithMany(a => a.Comments)
            .HasForeignKey("AuthorId")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        modelBuilder.Entity<Comment>()
            .HasOne<Cheep>(c => c.Cheep)
            .WithMany(c => c.Comments)
            .HasForeignKey("CheepId")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        modelBuilder.Entity<Like>()
            .HasOne<Author>(l => l.LikedByAuthor)
            .WithMany(a => a.Likes)
            .HasForeignKey("AuthorId")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        modelBuilder.Entity<Like>()
            .HasOne<Cheep>(l => l.Cheep)
            .WithMany(c => c.Likes)
            .HasForeignKey("CheepId")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}