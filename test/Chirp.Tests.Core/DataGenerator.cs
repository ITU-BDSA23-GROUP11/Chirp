using Bogus;
using Chirp.Infrastructure.Contexts;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using MockQueryable.Moq;
using Moq;

namespace Chirp.Tests.Core;

public class DataGenerator
{
    public struct AuthorCheepsData
    {
        public List<Author> Authors;
        public List<Cheep> Cheeps;
        public List<Like> Likes;
        public List<Comment> Comments;
    }

    public struct ChirpDbContextData
    {
        public List<Author> Authors;
        public List<Cheep> Cheeps;
        public List<Like> Likes;
        public List<Comment> Comments;
        public Mock<ChirpDbContext> MockChirpDbContext;
        public Mock<DbSet<Author>> MockDbAuthorsSet;
        public Mock<DbSet<Cheep>> MockDbCheepsSet;
        public Mock<DbSet<Like>> MockDbLikesSet;
        public Mock<DbSet<Comment>> MockDbCommentsSet;
    }
    
    public static Faker<Author> GenerateAuthorFaker(bool generateIds = true)
    {
        var authorsFaker = new Faker<Author>()
            .RuleFor(a => a.Name, f => f.Name.FullName())
            .RuleFor(a => a.Username, (f, a) => f.Internet.UserName(a.Name))
            .RuleFor(a => a.AvatarUrl, f => f.Internet.Avatar());

        if (generateIds)
        {
            authorsFaker.RuleFor(a => a.AuthorId, f => f.Random.Guid());
        }

        return authorsFaker;
    }
    
    public static Faker<Cheep> GenerateCheepFaker(List<Author> authors, bool generateIds = true)
    {
        var cheepsFaker = new Faker<Cheep>()
            .RuleFor(c => c.Text, f => f.Random.Words())
            .RuleFor(c => c.Timestamp, f => f.Date.Past())
            .RuleFor(c => c.Author, f => f.PickRandom(authors));

        if (generateIds)
        {
            cheepsFaker.RuleFor(c => c.CheepId, f => f.Random.Guid());
        }

        return cheepsFaker;
    }

    public static List<Author> GenerateFollows(List<Author> authors)
    {
        var faker = new Faker();
        var rand = new Random();
        foreach (Author author in authors)
        {
            for (int i = 0; i < rand.Next(3, 10); i++)
            {
                var followAuthor = faker.PickRandom(authors);
                if (followAuthor.AuthorId.ToString().Equals(author.AuthorId.ToString())) continue;
                author.Follows.Add(followAuthor);
                followAuthor.FollowedBy.Add(author);
            }
        }

        return authors;
    }
    
    public static Faker<Like> GenerateLikesFaker(List<Author> authors, List<Cheep> cheeps, bool generateIds = true)
    {
        var likesFaker = new Faker<Like>()
            .RuleFor(l => l.LikedByAuthor, f => f.PickRandom(authors))
            .RuleFor(l => l.Cheep, f => f.PickRandom(cheeps));
        if (generateIds)
        {
            likesFaker.RuleFor(c => c.LikeId, f => f.Random.Guid());
        }

        return likesFaker;
    }

    public static Faker<Comment> GenerateCommentsFaker(List<Author> authors, List<Cheep> cheeps, bool generateIds = true)
    {
        var commentsFaker = new Faker<Comment>()
            .RuleFor(c => c.CommentAuthor, f => f.PickRandom(authors))
            .RuleFor(c => c.Cheep, f => f.PickRandom(cheeps))
            .RuleFor(c => c.Timestamp, f => f.Date.Past())
            .RuleFor(c => c.Text, f => f.Random.Words());
        if (generateIds)
        {
            commentsFaker.RuleFor(c => c.CommentId, f => f.Random.Guid());
        }

        return commentsFaker;
    }
    
    public static AuthorCheepsData GenerateAuthorsAndCheeps(
        int minAuthors = 100,
        int maxAuthors = 200,
        int minCheeps = 500,
        int maxCheeps = 1000,
        int minLikes = 1000,
        int maxLikes = 2000,
        int minComments = 5,
        int maxComments = 100,
        bool generateIds = true
    )
    {
        var authorsFaker = GenerateAuthorFaker(generateIds);

        List<Author> authorsData = GenerateFollows(authorsFaker.GenerateBetween(minAuthors, maxAuthors));

        var cheepsFaker = GenerateCheepFaker(authorsData, generateIds);

        List<Cheep> cheepsData = cheepsFaker.GenerateBetween(minCheeps, maxCheeps);

        var likesFaker = GenerateLikesFaker(authorsData, cheepsData);

        List<Like> likesData = likesFaker.GenerateBetween(minLikes, maxLikes);

        var commentsFaker = GenerateCommentsFaker(authorsData, cheepsData, generateIds);

        List<Comment> commentsData = commentsFaker.GenerateBetween(minComments, maxComments);

        return new AuthorCheepsData
        {
            Authors = authorsData,
            Cheeps = cheepsData,
            Likes = likesData,
            Comments = commentsData
        };
    }

    public static ChirpDbContextData GenerateMockChirpDbContext(bool withErrorProvocation = false)
    {
        
        AuthorCheepsData data = GenerateAuthorsAndCheeps();

        var mockAuthors = data.Authors.AsQueryable().BuildMockDbSet();
        var mockCheeps = data.Cheeps.AsQueryable().BuildMockDbSet();
        var mockLikes = data.Likes.AsQueryable().BuildMockDbSet();
        var mockComments = data.Comments.AsQueryable().BuildMockDbSet();
        
        mockAuthors
            .Setup(m => m.Add(It.IsAny<Author>()))
            .Callback((Author author) =>
            {
                data.Authors.Add(author);
            });
        mockAuthors
            .Setup(m => m.Remove(It.IsAny<Author>()))
            .Callback((Author author) =>
            {
                data.Authors.Remove(author);
            });
        
        mockCheeps
            .Setup(m => m.Add(It.IsAny<Cheep>()))
            .Callback((Cheep cheep) =>
            {
                data.Cheeps.Add(cheep);
            });
        mockCheeps
            .Setup(m => m.Add(It.IsAny<Cheep>()))
            .Callback((Cheep cheep) =>
            {
                data.Cheeps.Remove(cheep);
            });
        mockLikes
            .Setup(m => m.Add(It.IsAny<Like>()))
            .Callback((Like like) =>
            {
                data.Likes.Add(like);
            });
        mockLikes
            .Setup(m => m.Add(It.IsAny<Like>()))
            .Callback((Like like) =>
            {
                data.Likes.Remove(like);
            });
        mockComments
            .Setup(m => m.Add(It.IsAny<Comment>()))
            .Callback((Comment comment) =>
            {
                data.Comments.Add(comment);
            });
        mockComments
            .Setup(m => m.Add(It.IsAny<Comment>()))
            .Callback((Comment comment) =>
            {
                data.Comments.Remove(comment);
            });
        
        var mockChirpDbContext = new Mock<ChirpDbContext>();
        if (!withErrorProvocation)
        {
            mockChirpDbContext.Setup(d => d.Authors).Returns(mockAuthors.Object);
            mockChirpDbContext.Setup(d => d.Cheeps).Returns(mockCheeps.Object);
            mockChirpDbContext.Setup(d => d.Likes).Returns(mockLikes.Object);
            mockChirpDbContext.Setup(d => d.Comments).Returns(mockComments.Object);
        }
        
        return new ChirpDbContextData
        {
            Authors = data.Authors,
            Cheeps = data.Cheeps,
            Likes = data.Likes,
            Comments = data.Comments,
            MockChirpDbContext = mockChirpDbContext,
            MockDbAuthorsSet = mockAuthors,
            MockDbCheepsSet = mockCheeps,
            MockDbLikesSet = mockLikes,
            MockDbCommentsSet = mockComments
        };
    }
    
    public class FakeUrlOriginAndReferer
    {
        public required string Origin;
        public required string Path;
        public required string Referer;
    }
    
    public static List<FakeUrlOriginAndReferer> GenerateUrlOriginAndReferer(
        int minAmount = 100,
        int maxAmount = 500
    )
    {
        var urlFaker = new Faker<FakeUrlOriginAndReferer>()
            .RuleFor(u => u.Origin, f => f.Internet.Url())
            .RuleFor(u => u.Path, f => f.Internet.UrlRootedPath())
            .RuleFor(u => u.Referer, (_, u) => u.Origin+u.Path);

        return urlFaker.GenerateBetween(minAmount, maxAmount);
    }
}