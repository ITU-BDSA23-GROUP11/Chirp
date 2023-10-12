using Chirp.DBService.Models;
using Chirp.DBService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chirp.DBService.Tests.Repositories;

public class CheepRepositoryTests
{
    private readonly ICheepRepository _cheepRepository;

    public CheepRepositoryTests()
    {
        ChirpDBContext context = new ChirpDBContext();
        context.Database.Migrate();
        DbInitializer.SeedDatabase(context);
        _cheepRepository = new CheepRepository(context);
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

        List<Cheep> cheeps = _cheepRepository.GetCheepsWithAuthors();

        _cheepRepository.AddCheep(cheep);

        List<Cheep> updatedCheeps = _cheepRepository.GetCheepsWithAuthors();
        
        Assert.Equal(cheeps.Count+1, updatedCheeps.Count);
        Assert.Equal(cheep, updatedCheeps.Last());
        Assert.Equal(cheep.CheepId, updatedCheeps.Last().CheepId);
        
        _cheepRepository.DeleteCheep(cheep);
    }

    [Fact]
    public void TestGetCheeps()
    {
        List<Cheep> cheeps = _cheepRepository.GetCheepsWithAuthors();
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
        List<Cheep> cheeps = _cheepRepository.GetCheepsFromAuthorNameWithAuthors(authorName);
        foreach (var cheep in cheeps)
        {
            Assert.Equal(authorName, cheep.Author.Name);
        }
    }
}