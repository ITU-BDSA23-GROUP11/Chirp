using Chirp.DBService.Models;
using Chirp.DBService.Repositories;
using Chirp.DBService.Tests.Fixtures;

namespace Chirp.DBService.Tests.Repositories;

public class CheepRepositoryTests : IClassFixture<ChirpDbContextFixture>
{
    private readonly ChirpDbContextFixture _fixture;

    public CheepRepositoryTests(ChirpDbContextFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestAddCheep()
    {
        ICheepRepository cheepRepository = new CheepRepository(_fixture.GetContext());
        Author author = new Author
        {
            Name = "Kim",
            Email = "kim@itu.dk"
        };
        Cheep cheep = new Cheep
        {
            Author = author
        };

        List<Cheep> cheeps = cheepRepository.GetCheepsWithAuthors();

        cheepRepository.AddCheep(cheep);

        List<Cheep> updatedCheeps = cheepRepository.GetCheepsWithAuthors();
    
        Assert.Equal(cheeps.Count+1, updatedCheeps.Count);
        Assert.Equal(cheep, updatedCheeps.Last());
        Assert.Equal(cheep.CheepId, updatedCheeps.Last().CheepId);
    
        cheepRepository.DeleteCheep(cheep);
    }

    [Fact]
    public void TestGetCheeps()
    {
        ICheepRepository cheepRepository = new CheepRepository(_fixture.GetContext());
        List<Cheep> cheeps = cheepRepository.GetCheepsWithAuthors();
        foreach (var cheep in cheeps)
        {
            Assert.NotNull(cheep);
            Assert.IsType<Cheep>(cheep);
        }
        Assert.Equal(12, cheeps.Count());
    }
    
    [Fact]
    public void TestGetCheepsFromAuthor()
    {
        ICheepRepository cheepRepository = new CheepRepository(_fixture.GetContext());
        var authorName = "Roger Histand";
        List<Cheep> cheeps = cheepRepository.GetCheepsFromAuthorNameWithAuthors(authorName);
        foreach (var cheep in cheeps)
        {
            Assert.Equal(authorName, cheep.Author.Name);
        }
        
        Assert.Equal(6, cheeps.Count());
    }

    [Fact]
    public void TestGetCheepCount()
    {
        ICheepRepository cheepRepository = new CheepRepository(_fixture.GetContext());

        int cheepsCount = cheepRepository.GetCheepCount();
        
        Assert.Equal(12, cheepsCount);
    }
    
    [Fact]
    public void TestGetCheepsForPage()
    {
        ICheepRepository cheepRepository = new CheepRepository(_fixture.GetContext());

        List<Cheep> cheeps = cheepRepository.GetCheepsForPage(1);
        
        Assert.Equal(12, cheeps.Count);
    }
    
    [Theory]
    [InlineData("Roger Histand", 6)]
    [InlineData("Luanna Muro", 6)]
    public void TestGetCheepsFromAuthorNameWithAuthors(string authorName, int authorCheepsAmount)
    {
        ICheepRepository cheepRepository = new CheepRepository(_fixture.GetContext());

        List<Cheep> cheeps = cheepRepository.GetCheepsFromAuthorNameWithAuthors(authorName);
        
        foreach (Cheep cheep in cheeps)
        {
            Assert.NotNull(cheep.Author);
            Assert.Equal(authorName, cheep.Author.Name);
        }
        
        Assert.Equal(authorCheepsAmount, cheeps.Count);
    }
    
    [Fact]
    public void TestGetCheepsFromAuthorNameForPage()
    {
        ICheepRepository cheepRepository = new CheepRepository(_fixture.GetContext());

        string authorName = "Roger Histand";
        List<Cheep> cheeps = cheepRepository.GetCheepsFromAuthorNameForPage(authorName, 1);
        
        foreach (Cheep cheep in cheeps)
        {
            Assert.NotNull(cheep.Author);
            Assert.Equal(authorName, cheep.Author.Name);
        }
        
        Assert.Equal(6, cheeps.Count);
    }
}