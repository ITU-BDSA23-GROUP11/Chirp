using Chirp.Infrastructure.Contexts;
using Chirp.Tests.Core;
using Microsoft.EntityFrameworkCore;
using Testcontainers.SqlEdge;

namespace Chirp.Infrastructure.Tests.Contexts;

public class ChirpDbContextFixture : IDisposable
{
    private readonly DbContextOptions<ChirpDbContext> _options;
    private readonly SqlEdgeContainer _container;
    public DataGenerator.AuthorCheepsData Data = DataGenerator.GenerateAuthorsAndCheeps(generateIds:false);

    public ChirpDbContextFixture()
    {
        // Start up test container
        _container = new SqlEdgeBuilder()
            .WithImage("mcr.microsoft.com/azure-sql-edge")
            .Build();
        
        _container.StartAsync().Wait();
        
        // Build options
        _options = new DbContextOptionsBuilder<ChirpDbContext>()
            .UseSqlServer(_container.GetConnectionString())
            .Options;

        // Add mock data to DB
        using var context = new ChirpDbContext(_options);
        if (context.Database.EnsureCreated())
        {   
            context.Authors.AddRange(Data.Authors);
            context.Cheeps.AddRange(Data.Cheeps);
            context.SaveChanges();
        }
    }

    public ChirpDbContext GetContext() => new (_options);
    
    public void Dispose()
    {
        _container.DisposeAsync().AsTask().Wait();
    }
}