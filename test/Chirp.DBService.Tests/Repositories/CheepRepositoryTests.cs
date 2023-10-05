using Chirp.DBService.Models;
using Chirp.DBService.Repositories;

namespace Chirp.DBService.Tests.Repositories;

public class CheepRepositoryTests
{
    private ICheepRepository _cheepRepository = new CheepRepository();

    [Fact]
    public void TestAddCheep()
    {
        Author author = new Author
        {
            Name = "Kim"
        };
        Cheep cheep = new Cheep
        {
            Author = author
        };

        List<Cheep> cheeps = _cheepRepository.GetCheeps();

        _cheepRepository.AddCheep(cheep);

        List<Cheep> updatedCheeps = _cheepRepository.GetCheeps();
        
        Assert.Equal(cheeps.Count+1, updatedCheeps.Count);
        Assert.Equal(cheep, updatedCheeps.Last());
        Assert.Equal(cheep.Id, updatedCheeps.Last().Id);
    }

    [Fact]
    public void TestGetCheeps()
    {
        List<Cheep> cheeps = _cheepRepository.GetCheeps();
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
        List<Cheep> cheeps = _cheepRepository.GetCheepsFromAuthorName(authorName);
        foreach (var cheep in cheeps)
        {
            Assert.Equal(authorName, cheep.Author.Name);
        }
    }
}