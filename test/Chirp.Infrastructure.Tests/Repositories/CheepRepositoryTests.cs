using Bogus;
using Chirp.Core.Dto;
using Chirp.Infrastructure.Models;
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
        Assert.Equal(author.Name, addedCheep.AuthorName);
        Assert.Equal(cheep.Text, addedCheep.Text);
        Assert.True(addedCheep.Timestamp.ToFileTimeUtc() > DateTime.UtcNow.Add(TimeSpan.FromSeconds(-1)).ToFileTimeUtc());
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
}