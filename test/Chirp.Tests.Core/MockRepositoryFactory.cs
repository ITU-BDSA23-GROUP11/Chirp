using Chirp.Core.Repositories;
using Chirp.Infrastructure.Contexts;
using Chirp.Infrastructure.Models;
using Chirp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Chirp.Tests.Core;

// Source: https://learn.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking

public struct MockChirpRepositories
{
    public ICheepRepository CheepRepository;
    public IAuthorRepository AuthorRepository;
    public List<Author> TestAuthors;
    public List<Cheep> TestCheeps;
    public Mock<ChirpDbContext> MockChirpDbContext;
    public Mock<DbSet<Author>> MockAuthorsDbSet;
    public Mock<DbSet<Cheep>> MockCheepsDbSet;
}

public static class MockRepositoryFactory
{
    public static MockChirpRepositories GetMockCheepRepository(bool withErrorProvocation = false)
    {
        var mockAuthorsDbSet = new Mock<DbSet<Author>>();
        var mockCheepsDbSet = new Mock<DbSet<Cheep>>();

        DataGenerator.AuthorCheepsData data = DataGenerator.GenerateAuthorsAndCheeps();
        
        // If mock db sets are not set up, it will throw an exception which will be caught by FetchWithErrorHandling
        if (!withErrorProvocation) {
            mockAuthorsDbSet.As<IQueryable<Author>>().Setup(m => m.Provider).Returns(data.Authors.AsQueryable().Provider);
            mockAuthorsDbSet.As<IQueryable<Author>>().Setup(m => m.Expression).Returns(data.Authors.AsQueryable().Expression);
            mockAuthorsDbSet.As<IQueryable<Author>>().Setup(m => m.ElementType).Returns(data.Authors.AsQueryable().ElementType);
            mockAuthorsDbSet.As<IQueryable<Author>>().Setup(m => m.GetEnumerator()).Returns(data.Authors.AsQueryable().GetEnumerator());
            
            mockCheepsDbSet.As<IQueryable<Cheep>>().Setup(m => m.Provider).Returns(data.Cheeps.AsQueryable().Provider);
            mockCheepsDbSet.As<IQueryable<Cheep>>().Setup(m => m.Expression).Returns(data.Cheeps.AsQueryable().Expression);
            mockCheepsDbSet.As<IQueryable<Cheep>>().Setup(m => m.ElementType).Returns(data.Cheeps.AsQueryable().ElementType);
            mockCheepsDbSet.As<IQueryable<Cheep>>().Setup(m => m.GetEnumerator()).Returns(data.Cheeps.AsQueryable().GetEnumerator());
        }
        
        var mockChirpDbContext = new Mock<ChirpDbContext>();
        mockChirpDbContext.Setup(m => m.Authors).Returns(mockAuthorsDbSet.Object);
        mockChirpDbContext.Setup(m => m.Cheeps).Returns(mockCheepsDbSet.Object);
        
        ICheepRepository cheepRepository = new CheepRepository(mockChirpDbContext.Object);
        IAuthorRepository authorRepository = new AuthorRepository(mockChirpDbContext.Object);

        return new MockChirpRepositories
        {
            CheepRepository = cheepRepository,
            AuthorRepository = authorRepository,
            TestAuthors = data.Authors,
            TestCheeps = data.Cheeps,
            MockChirpDbContext = mockChirpDbContext,
            MockAuthorsDbSet = mockAuthorsDbSet,
            MockCheepsDbSet = mockCheepsDbSet
        };
    }
}