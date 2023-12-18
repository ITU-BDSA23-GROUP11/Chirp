using Bogus;
using Chirp.Core.Dto;
using Chirp.Infrastructure.Models;
using Chirp.Tests.Core;
using Moq;

namespace Chirp.Infrastructure.Tests.Repositories;

public class CheepRepositoryTests
{
    private readonly MockChirpRepositories _mockChirpRepositories = MockRepositoryFactory.GetMockCheepRepositories();
    
    [Fact]
    public void TestAddCheep()
    {
        Author author = _mockChirpRepositories.TestAuthors.First();

        AddCheepDto cheep = new AddCheepDto
        {
            AuthorId = author.AuthorId,
            Text = new Faker().Random.Words()
        };
        
        CheepDto? addedCheep = _mockChirpRepositories.CheepRepository.AddCheep(cheep);
        
        _mockChirpRepositories.MockCheepsDbSet.Verify(m => m.Add(It.IsAny<Cheep>()), Times.Once);
        _mockChirpRepositories.MockChirpDbContext.Verify(m => m.SaveChanges(), Times.Once);
        Assert.Equal(Guid.Empty, addedCheep?.CheepId);
        Assert.Equal(author.Name, addedCheep?.AuthorName);
        Assert.Equal(cheep.Text, addedCheep?.Text);
        Assert.True(addedCheep?.Timestamp.ToFileTimeUtc() > DateTime.UtcNow.Add(TimeSpan.FromSeconds(-1)).ToFileTimeUtc());
    }
    
    [Fact]
    public void TestGetCheepCount()
    {
        int cheepsCount = _mockChirpRepositories.CheepRepository.GetCheepCount();
        Assert.Equal(_mockChirpRepositories.TestCheeps.Count, cheepsCount);
    }
    
    [Fact]
    public void TestGetCheepsForPage()
    {
        int pages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(_mockChirpRepositories.TestCheeps.Count) / 32));

        for (int i = 1; i <= pages; i++)
        {
            List<CheepDto> pageCheeps = _mockChirpRepositories.CheepRepository.GetCheepsForPage(i);

            if (i == pages)
            {
                Assert.Equal(_mockChirpRepositories.TestCheeps.Count%32, pageCheeps.Count);
            }
            else
            {
                Assert.Equal(32, pageCheeps.Count);
            }
        }
    }
    
    [Fact]
    public void TestGetAuthorCheepCount()
    {
        foreach (Author author in _mockChirpRepositories.TestAuthors)
        {
            int authorCheepCount = _mockChirpRepositories.CheepRepository.GetAuthorCheepCount(author.Username);
            Assert.Equal(_mockChirpRepositories.TestAuthors.Single(a => a.AuthorId == author.AuthorId).Cheeps.Count, authorCheepCount);
        }
    }
    
    [Fact]
    public void TestGetAuthorCheepsForPage()
    {
        foreach (Author author in _mockChirpRepositories.TestAuthors)
        {
            
            int pages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(author.Cheeps.Count) / 32));
            
            
            for (int i = 1; i <= pages; i++)
            {
                List<CheepDto> pageAuthorCheeps = _mockChirpRepositories.CheepRepository.GetAuthorCheepsForPage(author.Username, i);

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

    [Fact]
    public void TestCheepsSortedByTimestamp()
    {
        //Arrange
        int pages = Convert.ToInt32(
            Math.Ceiling(Convert.ToDecimal(_mockChirpRepositories.CheepRepository.GetCheepCount() / 32)));

        List<CheepDto> allCheeps = new List<CheepDto>();

        //Act
        for (int i = 1; i <= pages; i++)
        {
            allCheeps.AddRange(_mockChirpRepositories.CheepRepository.GetCheepsForPage(i));
        }
        
        //allCheeps should now hold all cheeps in a sorted order -> newest cheep first
        List<CheepDto> sorted = allCheeps.OrderByDescending(c => c.Timestamp).ToList(); 
        
        //Assert
        Assert.Equal(sorted, allCheeps);
    }

    [Fact]
    public void TestFetchWithErrorHandling()
    {
        var cheeps = MockRepositoryFactory
            .GetMockCheepRepositories(true)
            .CheepRepository
            .GetCheepsForPage(0);
        Assert.Empty(cheeps);
    }
}