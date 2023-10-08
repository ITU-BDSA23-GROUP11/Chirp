using Chirp.DBService.Models;

namespace Chirp.DBService.Tests;

public class ModelsIntegrationTests
{
    [Fact]
    public void TestAuthorAndCheepModelCorrectCreation()
    {
        var author = new Author
        {
            Name = "Kim",
            Email = "kim@itu.dk"
        };
        var cheep = new Cheep
        {
            Author = author,
            Message = "Hello Message"
        };
        
        Assert.Equal(author.Id, cheep.Author.Id);
        Assert.Single(author.Cheeps);
        Assert.Equal(cheep, author.Cheeps.First());
    }
}