using Chirp.DBService.Models;
using Chirp.DBService.Repositories;

namespace Chirp.DBService.Tests.Repositories;

public class CheepRepositoryFixture : IDisposable
{
    public readonly ICheepRepository CheepRepository = new CheepRepository(true);
    public void Dispose()
    {
        CheepRepository.DeleteDatabase();
    }

}

public class CheepRepositoryTests : IClassFixture<CheepRepositoryFixture>
{
    private readonly CheepRepositoryFixture _cheepRepositoryFixture;
    
    public CheepRepositoryTests(CheepRepositoryFixture cheepRepositoryFixture)
    {
        _cheepRepositoryFixture = cheepRepositoryFixture;
    }

    [Fact]
    public void TestAddCheep()
    {
        Author author = new Author
        {
            Name = "Kim",
            Email = "kim@itu.dk"
        };
        Cheep cheep = new Cheep
        {
            Author = author
        };

        List<Cheep> cheeps = _cheepRepositoryFixture.CheepRepository.GetCheepsWithAuthors();

        _cheepRepositoryFixture.CheepRepository.AddCheep(cheep);

        List<Cheep> updatedCheeps = _cheepRepositoryFixture.CheepRepository.GetCheepsWithAuthors();
        
        Assert.Equal(cheeps.Count+1, updatedCheeps.Count);
        Assert.Equal(cheep, updatedCheeps.Last());
        Assert.Equal(cheep.CheepId, updatedCheeps.Last().CheepId);
    }

    [Fact]
    public void TestGetCheeps()
    {
        List<Cheep> cheeps = _cheepRepositoryFixture.CheepRepository.GetCheepsWithAuthors();
        foreach (var cheep in cheeps)
        {
            Assert.NotNull(cheep);
            Assert.IsType<Cheep>(cheep);
        }
    }
    
    [Fact]
    public void TestGetCheepsFromAuthor()
    {
        var authorName = "Kim";
        List<Cheep> cheeps = _cheepRepositoryFixture.CheepRepository.GetCheepsFromAuthorNameWithAuthors(authorName);
        foreach (var cheep in cheeps)
        {
            Assert.Equal(authorName, cheep.Author.Name);
        }
    }
}