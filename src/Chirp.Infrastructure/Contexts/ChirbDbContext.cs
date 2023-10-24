using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Chirp.Infrastructure.Contexts;

// Source: https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli

public class ChirpDbContext : DbContext
{
    public virtual DbSet<Cheep> Cheeps { get; set; } = null!;
    public virtual DbSet<Author> Authors { get; set; } = null!;
    
    private readonly IConfiguration? _configuration;
    
    public ChirpDbContext()
    { }
    
    public ChirpDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ChirpDbContext(DbContextOptions<ChirpDbContext> options)
        : base(options)
    { }
    
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
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_configuration != null)
        {
            string rawConnectionString = (_configuration.GetConnectionString("ChirpDb") ?? "{TEMP_DIR}/chirp_db.db").Replace("{TEMP_DIR}", Path.GetTempPath());
            string connectionString = Path.DirectorySeparatorChar+Path.Join(rawConnectionString.Split("/"));
            optionsBuilder.UseSqlite($"Data Source={connectionString}", b => b.MigrationsAssembly("Chirp.Infrastructure"));
            Console.WriteLine("ChirpDBContext database initialised at: "+connectionString);
        }
        else
        {
            base.OnConfiguring(optionsBuilder);
        }
        
    }
}