using Chirp.DBService.Models;

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
    }

    [Fact]
    public void TestCheepFields()
    {
        Author author = new Author
        {
            Name = "Kim ITU",
            Email = "kim@itu.dk"
        };

        string text = "Test Message to the world";

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
        var author = new Author
        {
            Name = "Kim",
            Email = "kim@itu.dk"
        };
        var cheep = new Cheep
        {
            Author = author,
            Text = "Hello Message"
        };
        var timeNowMinusOneSecond = DateTime.UtcNow.Add(TimeSpan.FromSeconds(-1)).ToFileTimeUtc();
        var cheepTime = cheep.Timestamp.ToFileTimeUtc();
        
        Assert.True(cheepTime > timeNowMinusOneSecond);
    }
}