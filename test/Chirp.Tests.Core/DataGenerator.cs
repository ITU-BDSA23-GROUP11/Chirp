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

    }
    
    public static Faker<Author> GenerateAuthorFaker(bool generateIds = true)
    {
        var authorsFaker = new Faker<Author>()
            .RuleFor(a => a.Name, f => f.Name.FullName())
            .RuleFor(a => a.Email, (f, a) => f.Internet.Email(a.Name));

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
        bool generateIds = true
    )
    {
        var authorsFaker = GenerateAuthorFaker(generateIds);

        List<Author> authorsData = authorsFaker.GenerateBetween(minAuthors, maxAuthors);

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

    public class FakeClaims
    {
        public string? GivenName;
        public string? Surname;
        public string? Email;
        public Guid Id;
    }
    
    public static List<FakeClaims> GenerateUserClaims(
        int minAmount = 100,
        int maxAmount = 500
    )
    {
        var claimsFaker = new Faker<FakeClaims>()
            .RuleFor(c => c.GivenName, f => f.Random.Bool() ? f.Name.FirstName() : null)
            .RuleFor(c => c.Surname, f => f.Random.Bool() ? f.Name.LastName() : null)
            .RuleFor(c => c.Email,
                (f, c) => f.Random.Bool() ? f.Internet.Email(firstName: c.GivenName, lastName: c.Surname) : null)
            .RuleFor(c => c.Id, f => f.Random.Guid());

        return claimsFaker.GenerateBetween(minAmount, maxAmount);
    }
}