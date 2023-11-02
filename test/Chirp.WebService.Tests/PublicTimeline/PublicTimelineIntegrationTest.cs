using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Chirp.WebService.Tests.PublicTimeline;

public class PublicTimelineIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient usableClient;

    public PublicTimelineIntegrationTest(WebApplicationFactory<Program> _factory)
    {
        factory = _factory;
        usableClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            { AllowAutoRedirect = true, HandleCookies = true });
    }
    
    [Theory]
    //[InlineData("Helge")]
    [InlineData("https://www.itu.dk/")]
    public async void PrivateTimelineDisplayedTest(String url)
    {
        //Act
        var response = usableClient.GetAsync(url).Result.StatusCode;

        Console.WriteLine(response.ToString());
    }
}