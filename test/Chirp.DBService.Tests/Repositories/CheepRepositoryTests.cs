using Bogus;
using Chirp.DBService.Models;
using Chirp.DBService.Tests.Utilities;
using Moq;

namespace Chirp.DBService.Tests.Repositories;

public class CheepRepositoryTests
{
    private readonly MockCheepRepository _mockCheepRepository = MockRepositoryFactory.GetMockCheepRepository();

    [Fact]
    public void TestAddCheep()
    {
        Author author = _mockCheepRepository.TestAuthors.First();
        Cheep cheep = new Cheep
        {
            CheepId = new Faker().Random.Guid(),
            Author = author,
            Text = new Faker().Random.Words(),
            Timestamp = new Faker().Date.Past()
        };
        
        Cheep addedCheep = _mockCheepRepository.CheepRepository.AddCheep(cheep);
        
        _mockCheepRepository.MockCheepsDbSet.Verify(m => m.Add(It.IsAny<Cheep>()), Times.Once);
        _mockCheepRepository.MockChirpDbContext.Verify(m => m.SaveChanges(), Times.Once);
        Assert.Equal(cheep, addedCheep);
    }
    
    [Fact]
    public void TestDeleteCheep()
    {
        Author author = _mockCheepRepository.TestAuthors.First();
        Cheep cheep = author.Cheeps.First();

        _mockCheepRepository.CheepRepository.DeleteCheep(cheep);
        
        _mockCheepRepository.MockCheepsDbSet.Verify(m => m.Remove(It.IsAny<Cheep>()), Times.Once);
        _mockCheepRepository.MockChirpDbContext.Verify(m => m.SaveChanges(), Times.Once);
    }
    
    // TODO: Test Delete Cheep with non-DB cheep - how does CheepRepository handle it?
    
    [Fact]
    public void TestGetCheepCount()
    {
        int cheepsCount = _mockCheepRepository.CheepRepository.GetCheepCount();
        Assert.Equal(_mockCheepRepository.TestCheeps.Count, cheepsCount);
    }

    [Fact]
    public void TestGetCheepsWithAuthors()
    {
        List<Cheep> cheeps = _mockCheepRepository.CheepRepository.GetCheepsWithAuthors();
        
        Assert.Equal(_mockCheepRepository.TestCheeps.Count(), cheeps.Count());
        foreach (var cheep in cheeps)
        {
            Assert.NotNull(cheep);
            Assert.IsType<Cheep>(cheep);
            Assert.NotNull(cheep.Author);
        }
        
        foreach (Author author in _mockCheepRepository.TestAuthors)
        {
            Assert.Equal(author.Cheeps.Count, cheeps.Count(c => c.Author == author));
        }
    }
    
    [Fact]
    public void TestGetCheepsForPage()
    {
        int pages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(_mockCheepRepository.TestCheeps.Count) / 32));

        for (int i = 1; i <= pages; i++)
        {
            List<Cheep> pageCheeps = _mockCheepRepository.CheepRepository.GetCheepsForPage(i);

            if (i == pages)
            {
                Assert.Equal(_mockCheepRepository.TestCheeps.Count%32, pageCheeps.Count);
            }
            else
            {
                Assert.Equal(32, pageCheeps.Count);
            }
        }
    }
    
    [Fact]
    public void TestGetCheepsFromAuthorNameWithAuthors()
    {
        foreach (Author author in _mockCheepRepository.TestAuthors)
        {
            List<Cheep> authorCheeps = _mockCheepRepository.CheepRepository.GetCheepsFromAuthorNameWithAuthors(author.Name);
            foreach (Cheep cheep in authorCheeps)
            {
                Assert.NotNull(cheep.Author);
                Assert.Equal(author.Name, cheep.Author.Name);
            }
            
            Assert.Equal(author.Cheeps.Count, authorCheeps.Count);
        }
    }
    
    [Fact]
    public void TestGetCheepsFromAuthorNameForPage()
    {
        foreach (Author author in _mockCheepRepository.TestAuthors)
        {
            
            int pages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(author.Cheeps.Count) / 32));
            
            
            for (int i = 1; i <= pages; i++)
            {
                List<Cheep> pageAuthorCheeps = _mockCheepRepository.CheepRepository.GetCheepsFromAuthorNameForPage(author.Name, i);

                if (i == pages)
                {
                    Assert.Equal(author.Cheeps.Count%32, pageAuthorCheeps.Count);
                }
                else
                {
                    Assert.Equal(32, pageAuthorCheeps.Count);
                }
            }
        }
    }
}