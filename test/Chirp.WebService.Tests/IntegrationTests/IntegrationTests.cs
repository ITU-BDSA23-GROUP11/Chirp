using Chirp.Tests.Core.Fixtures;
using HtmlAgilityPack;

// Used parts of https://github.com/testcontainers/testcontainers-dotnet/tree/develop/examples/WeatherForecast

namespace Chirp.WebService.Tests.IntegrationTests;

public sealed class IntegrationTests : IClassFixture<WebApplicationFixture>
{
    private readonly HttpClient _httpClient;
    
    public IntegrationTests(WebApplicationFixture webApplicationFixture)
    {
        _httpClient = webApplicationFixture.GetClient().Client;
    }
    
    [Fact]  
    public async void CanEstablishConnection()
    {
        //Act
        var rsp = await _httpClient.GetAsync("/");
        rsp.EnsureSuccessStatusCode();
                
        //Assert
        Assert.Equal("OK", rsp.StatusCode.ToString());
    }
    
    [Fact]
    public async void FrontPageContains32Cheeps()
    {
        //Arrange & Act
        var rsp = await _httpClient.GetAsync("/");
        string htmlContent = await rsp.Content.ReadAsStringAsync();
        
        //Parse the htmlContent to a HTMLDocument
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);
        
        int amountOfListItems = doc.DocumentNode.SelectNodes("//*[@id='cheeps']").Count();
        
        Assert.Equal(32, amountOfListItems);
    }
        
    [Theory]
    [InlineData("Helge")]
    [InlineData("SampleUser")]
    public async void PrivateTimelinesAreDisplayed(String page)
    {
        //Act
        var rsp = await _httpClient.GetAsync("/" + page);
        string htmlContent = await rsp.Content.ReadAsStringAsync();
                
        //Check that the page contains the parameter name
        Assert.Contains(page, htmlContent);
    }
}