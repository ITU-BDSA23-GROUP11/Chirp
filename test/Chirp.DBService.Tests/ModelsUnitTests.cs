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
    }
}