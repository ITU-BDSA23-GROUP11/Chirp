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