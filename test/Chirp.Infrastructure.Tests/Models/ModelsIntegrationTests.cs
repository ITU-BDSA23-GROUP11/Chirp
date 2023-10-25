using Bogus;
using Chirp.Infrastructure.Models;
using Chirp.Infrastructure.Tests.Utilities;

namespace Chirp.Infrastructure.Tests.Models;

public class ModelsIntegrationTests
{
    [Fact]
    public void TestAuthorAndCheepModelCorrectCreation()
    {
        var author = DataGenerator.GenerateAuthorFaker().Generate();
        var cheep = new Cheep
        {
            Author = author,
            Text = new Faker().Random.Words(),
            Timestamp = new Faker().Date.Past()
        };
        
        Assert.Equal(author.AuthorId, cheep.Author.AuthorId);
        Assert.Single(author.Cheeps);
        Assert.Equal(cheep, author.Cheeps.First());
    }
}