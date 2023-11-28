using Bogus;
using Chirp.Core.Dto;
using Chirp.Infrastructure.Models;
using Chirp.Tests.Core;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Chirp.Infrastructure.Tests.Repositories;

public class CheepRepositoryTests
{
    private readonly MockCheepRepository _mockCheepRepository = MockRepositoryFactory.GetMockCheepRepository();

    [Fact]
    public void TestAddCheep()
    {
        Author author = _mockCheepRepository.TestAuthors.First();

        AddCheepDto cheep = new AddCheepDto
        {
            AuthorName = author.Name,
            AuthorEmail = author.Email,
            Text = new Faker().Random.Words()
        };
        
        CheepDto addedCheep = _mockCheepRepository.CheepRepository.AddCheep(cheep);
        
        _mockCheepRepository.MockCheepsDbSet.Verify(m => m.Add(It.IsAny<Cheep>()), Times.Once);
        _mockCheepRepository.MockChirpDbContext.Verify(m => m.SaveChanges(), Times.Once);
        Assert.Equal(Guid.Empty, addedCheep.CheepId);
        Assert.Equal(author.Name, addedCheep.AuthorName);
        Assert.Equal(cheep.Text, addedCheep.Text);
        Assert.True(addedCheep.Timestamp.ToFileTimeUtc() > DateTime.UtcNow.Add(TimeSpan.FromSeconds(-1)).ToFileTimeUtc());
    }

[Fact]
public void DeleteCheepTest()
{
    // Arrange
    Author author = _mockCheepRepository.TestAuthors.First();

    AddCheepDto cheep = new AddCheepDto
    {
        AuthorName = author.Name,
        AuthorEmail = author.Email,
        Text = new Faker().Random.Words()
    };

    CheepDto addedCheep = _mockCheepRepository.CheepRepository.AddCheep(cheep);

    // Act
    _mockCheepRepository.CheepRepository.DeleteCheep(addedCheep.CheepId.ToString(), addedCheep.AuthorName);
    
    // Assert
    _mockCheepRepository.MockCheepsDbSet.Verify(m => m.Remove(It.IsAny<Cheep>()), Times.Once);
    _mockCheepRepository.MockChirpDbContext.Verify(m => m.SaveChanges(), Times.Once);

    var cheepAfterDeletion = _mockCheepRepository.CheepRepository.GetCheepById(addedCheep.CheepId.ToString()); //TODO: Make method to get Cheep by id
    Assert.Null(cheepAfterDeletion);
}

    
    [Fact]
    public void TestGetCheepCount()
    {
        int cheepsCount = _mockCheepRepository.CheepRepository.GetCheepCount();
        Assert.Equal(_mockCheepRepository.TestCheeps.Count, cheepsCount);
    }
    
    [Fact]
    public void TestGetCheepsForPage()
    {
        int pages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(_mockCheepRepository.TestCheeps.Count) / 32));

        for (int i = 1; i <= pages; i++)
        {
            List<CheepDto> pageCheeps = _mockCheepRepository.CheepRepository.GetCheepsForPage(i);

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
    public void TestGetAuthorCheepCount()
    {
        foreach (Author author in _mockCheepRepository.TestAuthors)
        {
            int authorCheepCount = _mockCheepRepository.CheepRepository.GetAuthorCheepCount(author.Name);
            Assert.Equal(_mockCheepRepository.TestAuthors.Single(a => a.AuthorId == author.AuthorId).Cheeps.Count, authorCheepCount);
        }
    }
    
    [Fact]
    public void TestGetAuthorCheepsForPage()
    {
        foreach (Author author in _mockCheepRepository.TestAuthors)
        {
            
            int pages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(author.Cheeps.Count) / 32));
            
            
            for (int i = 1; i <= pages; i++)
            {
                List<CheepDto> pageAuthorCheeps = _mockCheepRepository.CheepRepository.GetAuthorCheepsForPage(author.Name, i);

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
            Math.Ceiling(Convert.ToDecimal(_mockCheepRepository.CheepRepository.GetCheepCount() / 32)));

        List<CheepDto> allCheeps = new List<CheepDto>();

        //Act
        for (int i = 1; i <= pages; i++)
        {
            allCheeps.AddRange(_mockCheepRepository.CheepRepository.GetCheepsForPage(i));
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
            .GetMockCheepRepository(true)
            .CheepRepository
            .GetCheepsForPage(0);
        Assert.Empty(cheeps);
    }
}
