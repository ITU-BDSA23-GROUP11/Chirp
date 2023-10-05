using Chirp.DBService.Models;

namespace Chirp.DBService.Tests;

public class ModelsUnitTests
{
    [Fact]
    public void TestAuthorModelUniqueIdCreation()
    {
        var author1 = new Author
        {
            Name = "Kim"
        };
        var author2 = new Author
        {
            Name = "Kim"
        };
        
        Assert.NotEqual(author1, author2);
        Assert.NotEqual(author1.Id, author2.Id);
        Assert.Equal(author1.Name, author2.Name);
    }
    
    [Fact]
    public void TestCheepModelUniqueIdCreation()
    {
        var author = new Author
        {
            Name = "Kim"
        };
        var cheep1 = new Cheep
        {
            Author = author,
            Message = "Hello Message"
        };
        var cheep2 = new Cheep
        {
            Author = author,
            Message = "Hello Message"
        };
        
        Assert.NotEqual(cheep1, cheep2);
        Assert.NotEqual(cheep1.Id, cheep2.Id);
        Assert.NotEqual("", cheep1.Message);
        Assert.Equal(cheep1.Message, cheep2.Message);
    }
    
    [Fact]
    public void TestCheepModelCorrectTimestamp()
    {
        var author = new Author
        {
            Name = "Kim"
        };
        var cheep = new Cheep
        {
            Author = author,
            Message = "Hello Message"
        };
        var timeNowMinusOneSecond = DateTime.UtcNow.Add(TimeSpan.FromSeconds(-1)).ToFileTimeUtc();
        var cheepTime = cheep.Timestamp.ToFileTimeUtc();
        
        Assert.True(cheepTime > timeNowMinusOneSecond);
    }
}