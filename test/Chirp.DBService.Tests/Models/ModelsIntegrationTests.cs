using Chirp.DBService.Models;

namespace Chirp.DBService.Tests.Models;

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
            Text = "Hello Message"
        };
        
        Assert.Equal(author.AuthorId, cheep.Author.AuthorId);
        Assert.Single(author.Cheeps);
        Assert.Equal(cheep, author.Cheeps.First());
    }
}