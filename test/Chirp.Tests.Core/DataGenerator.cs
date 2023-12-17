using Bogus;
using Chirp.Infrastructure.Models;

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

    public static Faker<Comment> GenerateCommentsFaker(List<Author> authors, List<Cheep> cheeps, bool generateIds = true)
    {
        var commentsFaker = new Faker<Comment>()
            .RuleFor(c => c.Author, f => f.PickRandom(authors))
            .RuleFor(c => c.Cheep, f => f.PickRandom(cheeps))
            .RuleFor(c => c.Text, f => f.Random.Words());
        if (generateIds)
        {
            commentsFaker.RuleFor(c => c.CommentId, f => f.Random.Guid());
        }

        return commentsFaker;
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
    
    public static Faker<Like> GenerateLikesFaker(List<Author> authors, List<Cheep> cheeps)
    {
        return new Faker<Like>()
            .RuleFor(l => l.LikedByAuthorId, f => f.PickRandom(authors).AuthorId)
            .RuleFor(l => l.CheepId, f => f.PickRandom(cheeps).CheepId);
    }
    
    public static AuthorCheepsData GenerateAuthorsAndCheeps(
        int minAuthors = 100,
        int maxAuthors = 200,
        int minCheeps = 500,
        int maxCheeps = 1000,
        int minLikes = 1000,
        int maxLikes = 2000,
        int minComments = 5,
        int maxComments = 10,
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