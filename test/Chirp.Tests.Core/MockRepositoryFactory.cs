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
    public ILikeRepository LikeRepository;
    public List<Author> TestAuthors;
    public List<Cheep> TestCheeps;
    public List<Like> TestLikes;
    public Mock<ChirpDbContext> MockChirpDbContext;
    public Mock<DbSet<Author>> MockAuthorsDbSet;
    public Mock<DbSet<Cheep>> MockCheepsDbSet;
    public Mock<DbSet<Like>> MockLikesDbSet;

}

public static class MockRepositoryFactory
{
    public static MockChirpRepositories GetMockCheepRepository(bool withErrorProvocation = false)
    {
        var mockAuthorsDbSet = new Mock<DbSet<Author>>();
        var mockCheepsDbSet = new Mock<DbSet<Cheep>>();
        var mockLikesDbSet = new Mock<DbSet<Like>>();
        
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

            mockLikesDbSet.As<IQueryable<Like>>().Setup(m => m.Provider).Returns(data.Likes.AsQueryable().Provider);
            mockLikesDbSet.As<IQueryable<Like>>().Setup(m => m.Expression).Returns(data.Likes.AsQueryable().Expression);
            mockLikesDbSet.As<IQueryable<Like>>().Setup(m => m.ElementType).Returns(data.Likes.AsQueryable().ElementType);
            mockLikesDbSet.As<IQueryable<Like>>().Setup(m => m.GetEnumerator()).Returns(data.Likes.AsQueryable().GetEnumerator());
        }
        
        var mockChirpDbContext = new Mock<ChirpDbContext>();
        mockChirpDbContext.Setup(m => m.Authors).Returns(mockAuthorsDbSet.Object);
        mockChirpDbContext.Setup(m => m.Cheeps).Returns(mockCheepsDbSet.Object);
        mockChirpDbContext.Setup(m => m.Likes).Returns(mockLikesDbSet.Object);

        IAuthorRepository authorRepository = new AuthorRepository(mockChirpDbContext.Object);
        ICheepRepository cheepRepository = new CheepRepository(mockChirpDbContext.Object, authorRepository);
        ILikeRepository likeRepository = new LikeRepository(mockChirpDbContext.Object);
        
        return new MockChirpRepositories
        {
            CheepRepository = cheepRepository,
            AuthorRepository = authorRepository,
            LikeRepository = likeRepository,
            TestAuthors = data.Authors,
            TestCheeps = data.Cheeps,
            TestLikes = data.Likes,
            MockChirpDbContext = mockChirpDbContext,
            MockAuthorsDbSet = mockAuthorsDbSet,
            MockCheepsDbSet = mockCheepsDbSet,
            MockLikesDbSet = mockLikesDbSet,
        };
    }
}