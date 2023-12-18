using Bogus;
using Chirp.Infrastructure.Contexts;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Chirp.Tests.Core;

public class DataGenerator
{
    public struct AuthorCheepsData
    {
        public List<Author> Authors;
        public List<Cheep> Cheeps;
        public List<Like> Likes;
    }

    public struct ChirpDbContextData
    {
        public List<Author> Authors;
        public List<Cheep> Cheeps;
        public List<Like> Likes;
        public Mock<ChirpDbContext> MockChirpDbContext;
        public Mock<DbSet<Author>> MockDbAuthorsSet;
        public Mock<DbSet<Cheep>> MockDbCheepsSet;
        public Mock<DbSet<Like>> MockDbLikesSet;
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
    
    public static AuthorCheepsData GenerateAuthorsAndCheeps(
        int minAuthors = 100,
        int maxAuthors = 200,
        int minCheeps = 500,
        int maxCheeps = 1000,
        int minLikes = 1000,
        int maxLikes = 2000,
        bool generateIds = true
    )
    {
        var authorsFaker = GenerateAuthorFaker(generateIds);

        List<Author> authorsData = GenerateFollows(authorsFaker.GenerateBetween(minAuthors, maxAuthors));

        var cheepsFaker = GenerateCheepFaker(authorsData, generateIds);

        List<Cheep> cheepsData = cheepsFaker.GenerateBetween(minCheeps, maxCheeps);

        var likesFaker = GenerateLikesFaker(authorsData, cheepsData);

        List<Like> likesData = likesFaker.GenerateBetween(minLikes, maxLikes);

        return new AuthorCheepsData
        {
            Authors = authorsData,
            Cheeps = cheepsData,
            Likes = likesData
        };
    }

    public static ChirpDbContextData GenerateMockChirpDbContext(bool withErrorProvocation = false)
    {
        var mockAuthorsDbSet = new Mock<DbSet<Author>>();
        var mockCheepsDbSet = new Mock<DbSet<Cheep>>();
        var mockLikesDbSet = new Mock<DbSet<Like>>();
        
        AuthorCheepsData data = GenerateAuthorsAndCheeps();
        
        // If mock db sets are not set up, it will throw an exception which will be caught by FetchWithErrorHandling
        if (!withErrorProvocation) {
            mockAuthorsDbSet.As<IQueryable<Author>>().Setup(m => m.Provider).Returns(data.Authors.AsQueryable().Provider);
            mockAuthorsDbSet.As<IQueryable<Author>>().Setup(m => m.Expression).Returns(data.Authors.AsQueryable().Expression);
            mockAuthorsDbSet.As<IQueryable<Author>>().Setup(m => m.ElementType).Returns(data.Authors.AsQueryable().ElementType);
            mockAuthorsDbSet.As<IQueryable<Author>>().Setup(m => m.GetEnumerator()).Returns(data.Authors.AsQueryable().GetEnumerator());
            mockAuthorsDbSet
                .Setup(m => m.Add(It.IsAny<Author>()))
                .Callback((Author author) =>
                {
                    data.Authors.Add(author);
                });
            mockAuthorsDbSet
                .Setup(m => m.Remove(It.IsAny<Author>()))
                .Callback((Author author) =>
                {
                    data.Authors.Remove(author);
                });
            
            mockCheepsDbSet.As<IQueryable<Cheep>>().Setup(m => m.Provider).Returns(data.Cheeps.AsQueryable().Provider);
            mockCheepsDbSet.As<IQueryable<Cheep>>().Setup(m => m.Expression).Returns(data.Cheeps.AsQueryable().Expression);
            mockCheepsDbSet.As<IQueryable<Cheep>>().Setup(m => m.ElementType).Returns(data.Cheeps.AsQueryable().ElementType);
            mockCheepsDbSet.As<IQueryable<Cheep>>().Setup(m => m.GetEnumerator()).Returns(data.Cheeps.AsQueryable().GetEnumerator());
            mockCheepsDbSet
                .Setup(m => m.Add(It.IsAny<Cheep>()))
                .Callback((Cheep cheep) =>
                {
                    data.Cheeps.Add(cheep);
                });
            mockCheepsDbSet
                .Setup(m => m.Add(It.IsAny<Cheep>()))
                .Callback((Cheep cheep) =>
                {
                    data.Cheeps.Remove(cheep);
                });


            mockLikesDbSet.As<IQueryable<Like>>().Setup(m => m.Provider).Returns(data.Likes.AsQueryable().Provider);
            mockLikesDbSet.As<IQueryable<Like>>().Setup(m => m.Expression).Returns(data.Likes.AsQueryable().Expression);
            mockLikesDbSet.As<IQueryable<Like>>().Setup(m => m.ElementType).Returns(data.Likes.AsQueryable().ElementType);
            mockLikesDbSet.As<IQueryable<Like>>().Setup(m => m.GetEnumerator()).Returns(data.Likes.AsQueryable().GetEnumerator());
            mockLikesDbSet
                .Setup(m => m.Add(It.IsAny<Like>()))
                .Callback((Like like) =>
                {
                    data.Likes.Add(like);
                });
            mockLikesDbSet
                .Setup(m => m.Add(It.IsAny<Like>()))
                .Callback((Like like) =>
                {
                    data.Likes.Remove(like);
                });
        }
        
        var mockChirpDbContext = new Mock<ChirpDbContext>();
        mockChirpDbContext.Setup(m => m.Authors).Returns(mockAuthorsDbSet.Object);
        mockChirpDbContext.Setup(m => m.Cheeps).Returns(mockCheepsDbSet.Object);
        mockChirpDbContext.Setup(m => m.Likes).Returns(mockLikesDbSet.Object);

        return new ChirpDbContextData
        {
            Authors = data.Authors,
            Cheeps = data.Cheeps,
            Likes = data.Likes,
            MockChirpDbContext = mockChirpDbContext,
            MockDbAuthorsSet = mockAuthorsDbSet,
            MockDbCheepsSet = mockCheepsDbSet,
            MockDbLikesSet = mockLikesDbSet
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