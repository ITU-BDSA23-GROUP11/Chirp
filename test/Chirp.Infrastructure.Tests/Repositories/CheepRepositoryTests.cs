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
    public async Task TestAddCheep()
    {
        //Arrange
        Author author = _mockChirpRepositories.TestAuthors.First();

        AddCheepDto cheep = new AddCheepDto
        {
            AuthorId = author.AuthorId,
            Text = new Faker().Random.Words()
        };
        //Act
        CheepDto? addedCheep = await _mockChirpRepositories.CheepRepository.AddCheep(cheep);
        //Assert
        _mockChirpRepositories.MockCheepsDbSet.Verify(m => m.Add(It.IsAny<Cheep>()), Times.Once);
        _mockChirpRepositories.MockChirpDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(Guid.Empty, addedCheep?.CheepId);
        Assert.Equal(author.Name, addedCheep?.AuthorName);
        Assert.Equal(cheep.Text, addedCheep?.Text);
        Assert.True(addedCheep?.Timestamp.ToFileTimeUtc() > DateTime.UtcNow.Add(TimeSpan.FromSeconds(-1)).ToFileTimeUtc());
    }

    [Fact]
    public async Task DeleteCheepTest()
    {
        //Arrange
        Cheep cheep = _mockChirpRepositories.TestCheeps.First();
        Guid cheepId = cheep.CheepId;
        //Act
        await _mockChirpRepositories.CheepRepository.DeleteCheep(cheepId);
        //Assert
        _mockChirpRepositories.MockCheepsDbSet.Verify(m => m.Remove(It.IsAny<Cheep>()), Times.Once);
        _mockChirpRepositories.MockChirpDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task TestGetCheepCount()
    {
        int cheepsCount = await _mockChirpRepositories.CheepRepository.GetCheepCount();
        Assert.Equal(_mockChirpRepositories.TestCheeps.Count, cheepsCount);
    }
    
    [Fact]
    public async Task TestGetCheepsForPage()
    {
        int pages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(_mockChirpRepositories.TestCheeps.Count) / 32));

        for (int i = 1; i <= pages; i++)
        {
            List<CheepDto> pageCheeps = await _mockChirpRepositories.CheepRepository.GetCheepsForPage(i);

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
    public async Task TestGetAuthorCheepCount()
    {
        foreach (Author author in _mockChirpRepositories.TestAuthors)
        {
            int authorCheepCount = await _mockChirpRepositories.CheepRepository.GetAuthorCheepCount(author.Username);
            Assert.Equal(_mockChirpRepositories.TestAuthors.Single(a => a.AuthorId == author.AuthorId).Cheeps.Count, authorCheepCount);
        }
    }
    
    [Fact]
    public async Task TestGetAuthorCheepsForPage()
    {
        foreach (Author author in _mockChirpRepositories.TestAuthors)
        {
            
            int pages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(author.Cheeps.Count) / 32));
            
            
            for (int i = 1; i <= pages; i++)
            {
                List<CheepDto> pageAuthorCheeps = await _mockChirpRepositories.CheepRepository.GetAuthorCheepsForPage(author.Username, i);

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
    public async Task TestCheepsSortedByTimestamp()
    {
        //Arrange
        int pages = Convert.ToInt32(
            Math.Ceiling(Convert.ToDecimal(await _mockChirpRepositories.CheepRepository.GetCheepCount() / 32)));

        List<CheepDto> allCheeps = new List<CheepDto>();

        //Act
        for (int i = 1; i <= pages; i++)
        {
            allCheeps.AddRange(await _mockChirpRepositories.CheepRepository.GetCheepsForPage(i));
        }
        
        //allCheeps should now hold all cheeps in a sorted order -> newest cheep first
        List<CheepDto> sorted = allCheeps.OrderByDescending(c => c.Timestamp).ToList(); 
        
        //Assert
        Assert.Equal(sorted, allCheeps);
    }

    [Fact]
    public async Task TestFetchWithErrorHandling()
    {
        var cheeps = await MockRepositoryFactory
            .GetMockCheepRepositories(true)
            .CheepRepository
            .GetCheepsForPage(0);
        Assert.Empty(cheeps);
    }

    [Fact]
    public async Task TestGetCheepsFromIds()
    {
        //Arrange
        List<Cheep> cheeps = _mockChirpRepositories.TestCheeps;
        HashSet<Guid> cheepIds = new HashSet<Guid>();
        for (var i = 0; i < 10; i++)
        {
            cheepIds.Add(cheeps[i].CheepId);
        }
        //Act
        List<CheepDto> cheepsFromId = await _mockChirpRepositories.CheepRepository.GetCheepsFromIds(cheepIds);
        
        //Assert
        int expectedAmount = 10;
        Assert.Equal(expectedAmount, cheepsFromId.Count);
    }

    [Fact]
    public async Task TestGetAuthorCheepsForPageAsOwner()
    {
        //Arrange
        Guid authorId = _mockChirpRepositories.TestCheeps.First().Author.AuthorId;
        //Act
        List<CheepDto> actualList = await _mockChirpRepositories.CheepRepository.GetAuthorCheepsForPageAsOwner(authorId, 1);
        //Assert
        Assert.NotEmpty(actualList);
    }
}