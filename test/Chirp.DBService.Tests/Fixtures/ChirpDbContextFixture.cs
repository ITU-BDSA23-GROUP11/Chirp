using Chirp.DBService.Contexts;
using Chirp.DBService.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.DBService.Tests.Fixtures;

public class ChirpDbContextFixture : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ChirpDbContext> _options;

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
            var a1 = new Author() { Name = "Roger Histand", Email = "Roger+Histand@hotmail.com" };
            var a2 = new Author() { Name = "Luanna Muro", Email = "Luanna-Muro@ku.dk" };

            var authors = new List<Author>() { a1, a2 };
            
            var c1 = new Cheep() { Author = a1, Text = "They were married in Chicago, with old Smith, and was expected aboard every day; meantime, the two went past me.", Timestamp = DateTime.Parse("2023-08-01 13:14:37") };
            var c2 = new Cheep() { Author = a1, Text = "And then, as he listened to all that''s left o'' twenty-one people.", Timestamp = DateTime.Parse("2023-08-01 13:15:21") };
            var c3 = new Cheep() { Author = a1, Text = "In various enchanted attitudes, like the Sperm Whale.", Timestamp = DateTime.Parse("2023-08-01 13:14:58") };
            var c4 = new Cheep() { Author = a1, Text = "Unless we succeed in establishing ourselves in some monomaniac way whatever significance might lurk in them.", Timestamp = DateTime.Parse("2023-08-01 13:14:34") };
            var c5 = new Cheep() { Author = a1, Text = "At last we came back!", Timestamp = DateTime.Parse("2023-08-01 13:14:35") };
            var c6 = new Cheep() { Author = a1, Text = "At first he had only exchanged one trouble for another.", Timestamp = DateTime.Parse("2023-08-01 13:14:13") };
            var c7 = new Cheep() { Author = a2, Text = "In the first watch, and every creditor paid in full.", Timestamp = DateTime.Parse("2023-08-01 13:16:13") };
            var c8 = new Cheep() { Author = a2, Text = "It was but a very ancient cluster of blocks generally painted green, and for no other, he shielded me.", Timestamp = DateTime.Parse("2023-08-01 13:14:01") };
            var c9 = new Cheep() { Author = a2, Text = "The folk on trust in me!", Timestamp = DateTime.Parse("2023-08-01 13:15:30") };
            var c10 = new Cheep() { Author = a2, Text = "It is a damp, drizzly November in my pocket, and switching it backward and forward with a most suspicious aspect.", Timestamp = DateTime.Parse("2023-08-01 13:13:34") };
            var c11 = new Cheep() { Author = a2, Text = "I had no difficulty in finding where Sholto lived, and take it and in Canada.", Timestamp = DateTime.Parse("2023-08-01 13:14:11") };
            var c12 = new Cheep() { Author = a2, Text = "What did they take?", Timestamp = DateTime.Parse("2023-08-01 13:14:44") };

            var cheeps = new List<Cheep>() { c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12 };
            
            context.Authors.AddRange(authors);
            context.Cheeps.AddRange(cheeps);
            context.SaveChanges();
        }
    }

    public ChirpDbContext GetContext() => new (_options);
    
    public void Dispose() => _connection.Close();
}