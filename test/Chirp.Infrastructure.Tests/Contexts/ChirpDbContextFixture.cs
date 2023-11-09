using Chirp.Infrastructure.Contexts;
using Chirp.Tests.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Tests.Contexts;

public class ChirpDbContextFixture : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ChirpDbContext> _options;
    public DataGenerator.AuthorCheepsData Data = DataGenerator.GenerateAuthorsAndCheeps(generateIds:false);

    public ChirpDbContextFixture()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        _options = new DbContextOptionsBuilder<ChirpDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new ChirpDbContext(_options);
        if (context.Database.EnsureCreated())
        {   
            context.Authors.AddRange(Data.Authors);
            context.Cheeps.AddRange(Data.Cheeps);
            context.SaveChanges();
        }
    }

    public ChirpDbContext GetContext() => new (_options);
    
    public void Dispose() => _connection.Close();
}