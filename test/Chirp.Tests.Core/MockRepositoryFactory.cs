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
    public ICommentRepository CommentRepository;
    public List<Author> TestAuthors;
    public List<Cheep> TestCheeps;
    public List<Like> TestLikes;
    public List<Comment> TestComment;
    public Mock<ChirpDbContext> MockChirpDbContext;
    public Mock<DbSet<Author>> MockAuthorsDbSet;
    public Mock<DbSet<Cheep>> MockCheepsDbSet;
    public Mock<DbSet<Like>> MockLikesDbSet;
    public Mock<DbSet<Comment>> MockCommentsDbSet;

}

public static class MockRepositoryFactory
{
    public static MockChirpRepositories GetMockCheepRepositories(bool withErrorProvocation = false)
    {
        var mockChirpDbContext = DataGenerator.GenerateMockChirpDbContext(withErrorProvocation);

        IAuthorRepository authorRepository = new AuthorRepository(mockChirpDbContext.MockChirpDbContext.Object);
        ICheepRepository cheepRepository = new CheepRepository(mockChirpDbContext.MockChirpDbContext.Object, authorRepository);
        ILikeRepository likeRepository = new LikeRepository(mockChirpDbContext.MockChirpDbContext.Object);
        ICommentRepository commentRepository = new CommentRepository(mockChirpDbContext.MockChirpDbContext.Object);
        
        return new MockChirpRepositories
        {
            CheepRepository = cheepRepository,
            AuthorRepository = authorRepository,
            LikeRepository = likeRepository,
            CommentRepository = commentRepository,
            TestAuthors = mockChirpDbContext.Authors,
            TestCheeps = mockChirpDbContext.Cheeps,
            TestLikes = mockChirpDbContext.Likes,
            TestComment = mockChirpDbContext.Comments,
            MockChirpDbContext = mockChirpDbContext.MockChirpDbContext,
            MockAuthorsDbSet = mockChirpDbContext.MockDbAuthorsSet,
            MockCheepsDbSet = mockChirpDbContext.MockDbCheepsSet,
            MockLikesDbSet = mockChirpDbContext.MockDbLikesSet,
            MockCommentsDbSet = mockChirpDbContext.MockDbCommentsSet
        };
    }
}