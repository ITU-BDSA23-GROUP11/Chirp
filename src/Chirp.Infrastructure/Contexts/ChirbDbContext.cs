using System.Diagnostics.CodeAnalysis;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite.Infrastructure.Internal;
using Microsoft.Extensions.Configuration;

namespace Chirp.Infrastructure.Contexts;

// Source: https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli

public class ChirpDbContext : DbContext
{
    public virtual DbSet<Cheep> Cheeps { get; set; } = null!;
    public virtual DbSet<Author> Authors { get; set; } = null!;
    
    private readonly IConfiguration? _configuration;
    
    [ExcludeFromCodeCoverage]
    public ChirpDbContext()
    { }
    
    [ExcludeFromCodeCoverage]
    public ChirpDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [ExcludeFromCodeCoverage]
    public ChirpDbContext(DbContextOptions<ChirpDbContext> options)
        : base(options)
    { }
    
    [ExcludeFromCodeCoverage]
    public ChirpDbContext(DbContextOptions<ChirpDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cheep>()
            .HasOne<Author>(c => c.Author)
            .WithMany(a => a.Cheeps)
            .HasForeignKey("AuthorId")
            .IsRequired();
        modelBuilder.Entity<Author>().Property(a => a.Name).HasMaxLength(50).IsRequired(); //Restriction of maximum length
        modelBuilder.Entity<Author>().Property(e => e.Email).HasMaxLength(100).IsRequired(); //Restriction of maximum length
        modelBuilder.Entity<Author>().HasIndex(a => a.Name).IsUnique(); //Restrictions of uniqueness
    }

    [ExcludeFromCodeCoverage]
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_configuration != null)
        {
            string rawConnectionString = (_configuration.GetConnectionString("ChirpDb") ?? "{TEMP_DIR}/chirp_db.db").Replace("{TEMP_DIR}", Path.GetTempPath());
            string connectionString = Path.DirectorySeparatorChar+Path.Join(rawConnectionString.Split("/"));
            optionsBuilder.UseSqlite($"Data Source={connectionString}", b => b.MigrationsAssembly("Chirp.Infrastructure"));
            Console.WriteLine("ChirpDBContext database initialised at: "+connectionString);
        }
        else if (optionsBuilder.Options.FindExtension<SqliteOptionsExtension>() != null)
        {
            base.OnConfiguring(optionsBuilder);
        }
        else
        {
            optionsBuilder.UseSqlite("Filesystem:memory:");
        }
        
    }
    
    
}