using Bogus;

namespace Chirp.WebService.Tests.Utilities;

public static class DataGenerator
{
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
    }
    
    public static List<FakeClaims> GenerateUserClaims(
        int minAmount = 100,
        int maxAmount = 500
    )
    {
        var claimsFaker = new Faker<FakeClaims>()
            .RuleFor(c => c.GivenName, f => f.Random.Bool() ? f.Name.FirstName() : null)
            .RuleFor(c => c.Surname, f => f.Random.Bool() ? f.Name.LastName() : null)
            .RuleFor(c => c.Email, (f, c) => f.Random.Bool() ? f.Internet.Email(firstName: c.GivenName, lastName: c.Surname) : null);

        return claimsFaker.GenerateBetween(minAmount, maxAmount);
    }
}