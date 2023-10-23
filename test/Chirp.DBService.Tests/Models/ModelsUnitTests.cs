using Bogus;
using Chirp.DBService.Models;
using Chirp.DBService.Tests.Utilities;

namespace Chirp.DBService.Tests.Models;

public class ModelsUnitTests
{
    [Fact]
    public void TestAuthorFields()
    {
        string name = "Kim ITU";
        string email = "kim@itu.dk";
        
        Author author = new Author
        {
            Name = name,
            Email = email
        };
        
        Assert.Equal(name, author.Name);
        Assert.Equal(email, author.Email);
        Assert.Equal(Guid.Empty, author.AuthorId);
        Assert.Empty(author.Cheeps);
    }

    [Fact]
    public void TestCheepFields()
    {
        Author author = DataGenerator.GenerateAuthorFaker().Generate();

        string text = new Faker().Random.Words();

        Cheep cheep = new Cheep
        {
            Author = author,
            Text = text
        };
        
        Assert.Equal(text, cheep.Text);
        Assert.Equal(author.Name, cheep.Author.Name);
    }
    
    [Fact]
    public void TestCheepModelCorrectTimestamp()
    {
        var author = DataGenerator.GenerateAuthorFaker().Generate();
        var cheep = new Cheep
        {
            Author = author,
            Text = new Faker().Random.Words()
        };
        var timeNowMinusOneSecond = DateTime.UtcNow.Add(TimeSpan.FromSeconds(-1)).ToFileTimeUtc();
        var cheepTime = cheep.Timestamp.ToFileTimeUtc();
        
        Assert.True(cheepTime > timeNowMinusOneSecond);
    }
}