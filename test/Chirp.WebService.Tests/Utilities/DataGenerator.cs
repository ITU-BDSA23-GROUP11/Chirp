using Bogus;

namespace Chirp.WebService.Tests.Utilities;

public static class DataGenerator
{
    public class UrlOriginAndReferer
    {
        public required string Origin;
        public required string Path;
        public required string Referer;
    }
    
    public static List<UrlOriginAndReferer> GenerateUrlOriginAndReferer(
        int minAmount = 100,
        int maxAmount = 500
    )
    {
        var urlFaker = new Faker<UrlOriginAndReferer>()
            .RuleFor(u => u.Origin, f => f.Internet.Url())
            .RuleFor(u => u.Path, f => f.Internet.UrlRootedPath())
            .RuleFor(u => u.Referer, (_, u) => u.Origin+u.Path);

        return urlFaker.GenerateBetween(minAmount, maxAmount);
    }
}