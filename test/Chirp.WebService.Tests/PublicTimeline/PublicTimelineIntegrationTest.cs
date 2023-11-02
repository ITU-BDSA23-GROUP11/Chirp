using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;
using System.Net.Http;
using HtmlAgilityPack;

namespace Chirp.WebService.Tests.PublicTimeline;

public class PublicTimelineIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient usableClient;
    private readonly ITestOutputHelper output;//For debug purposes

    public PublicTimelineIntegrationTest(WebApplicationFactory<Program> _factory, ITestOutputHelper _output)
    {
        factory = _factory;
        usableClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            { AllowAutoRedirect = true, HandleCookies = true });
        this.output = _output;
    }

    [Fact]  
    public async void CanEstablishConnection()
    {
        //Act
        var rsp = await usableClient.GetAsync("/");
        rsp.EnsureSuccessStatusCode();
        
        //Assert
        Assert.Equal("OK", rsp.StatusCode.ToString());
    }

    [Fact]
    public async void FrontPageContains32Cheeps()
    {
        //Arrange & Act
        var rsp = await usableClient.GetAsync("/");
        string htmlContent = await rsp.Content.ReadAsStringAsync();

        //Parse the htmlContent to a HTMLDocument
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);

        int amountOfListItems = doc.DocumentNode.SelectNodes("//li").Count();

        Assert.Equal(32, amountOfListItems);
    }
    
    
    
}