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
    public static MockChirpRepositories GetMockCheepRepositories(bool withErrorProvocation = false)
    {
        var mockChirpDbContext = DataGenerator.GenerateMockChirpDbContext(withErrorProvocation);

        IAuthorRepository authorRepository = new AuthorRepository(mockChirpDbContext.MockChirpDbContext.Object);
        ICheepRepository cheepRepository = new CheepRepository(mockChirpDbContext.MockChirpDbContext.Object, authorRepository);
        ILikeRepository likeRepository = new LikeRepository(mockChirpDbContext.MockChirpDbContext.Object);
        
        return new MockChirpRepositories
        {
            CheepRepository = cheepRepository,
            AuthorRepository = authorRepository,
            LikeRepository = likeRepository,
            TestAuthors = mockChirpDbContext.Authors,
            TestCheeps = mockChirpDbContext.Cheeps,
            TestLikes = mockChirpDbContext.Likes,
            MockChirpDbContext = mockChirpDbContext.MockChirpDbContext,
            MockAuthorsDbSet = mockChirpDbContext.MockDbAuthorsSet,
            MockCheepsDbSet = mockChirpDbContext.MockDbCheepsSet,
            MockLikesDbSet = mockChirpDbContext.MockDbLikesSet,
        };
    }
}