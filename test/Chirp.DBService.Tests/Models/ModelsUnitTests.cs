using Chirp.DBService.Models;

namespace Chirp.DBService.Tests;

public class ModelsUnitTests
{
    [Fact]
    public void TestAuthorModelUniqueIdCreation()
    {
        var author1 = new Author
        {
            Name = "Kim",
            Email = "kim@itu.dk"
        };
        var author2 = new Author
        {
            Name = "Kim",
            Email = "kim@itu.dk"
        };
        
        Assert.NotEqual(author1, author2);
        Assert.NotEqual(author1.AuthorId, author2.AuthorId);
        Assert.Equal(author1.Name, author2.Name);
        Assert.Equal(author1.Email, author2.Email);
    }
    
    [Fact]
    public void TestCheepModelUniqueIdCreation()
    {
        var author = new Author
        {
            Name = "Kim",
            Email = "kim@itu.dk"
        };
        var cheep1 = new Cheep
        {
            Author = author,
            Text = "Hello Message"
        };
        var cheep2 = new Cheep
        {
            Author = author,
            Text = "Hello Message"
        };
        
        Assert.NotEqual(cheep1, cheep2);
        Assert.NotEqual(cheep1.CheepId, cheep2.CheepId);
        Assert.NotEqual("", cheep1.Text);
        Assert.Equal(cheep1.Text, cheep2.Text);
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