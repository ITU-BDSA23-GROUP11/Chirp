using Bogus;
using Chirp.DBService.Contexts;
using Chirp.DBService.Models;
using Chirp.DBService.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Chirp.DBService.Tests.Utilities;

public struct MockCheepRepository
{
    public ICheepRepository CheepRepository;
    public List<Author> TestAuthors;
    public List<Cheep> TestCheeps;
    public Mock<ChirpDbContext> MockChirpDbContext;
    public Mock<DbSet<Author>> MockAuthorsDbSet;
    public Mock<DbSet<Cheep>> MockCheepsDbSet;
}

public static class MockRepositoryFactory
{
    public static MockCheepRepository GetMockCheepRepository()
    {
        var mockAuthorsDbSet = new Mock<DbSet<Author>>();
        var mockCheepsDbSet = new Mock<DbSet<Cheep>>();

        DataGenerator.AuthorCheepsData data = DataGenerator.GenerateAuthorsAndCheeps();
        
        mockCheepsDbSet.As<IQueryable<Cheep>>().Setup(m => m.Provider).Returns(data.Cheeps.AsQueryable().Provider);
        mockCheepsDbSet.As<IQueryable<Cheep>>().Setup(m => m.Expression).Returns(data.Cheeps.AsQueryable().Expression);
        mockCheepsDbSet.As<IQueryable<Cheep>>().Setup(m => m.ElementType).Returns(data.Cheeps.AsQueryable().ElementType);
        mockCheepsDbSet.As<IQueryable<Cheep>>().Setup(m => m.GetEnumerator()).Returns(() => data.Cheeps.AsQueryable().GetEnumerator());
        
        var mockChirpDbContext = new Mock<ChirpDbContext>();
        mockChirpDbContext.Setup(m => m.Authors).Returns(mockAuthorsDbSet.Object);
        mockChirpDbContext.Setup(m => m.Cheeps).Returns(mockCheepsDbSet.Object);
        
        ICheepRepository cheepRepository = new CheepRepository(mockChirpDbContext.Object);

        return new MockCheepRepository
        {
            CheepRepository = cheepRepository,
            TestAuthors = data.Authors,
            TestCheeps = data.Cheeps,
            MockChirpDbContext = mockChirpDbContext,
            MockAuthorsDbSet = mockAuthorsDbSet,
            MockCheepsDbSet = mockCheepsDbSet
        };
    }
}