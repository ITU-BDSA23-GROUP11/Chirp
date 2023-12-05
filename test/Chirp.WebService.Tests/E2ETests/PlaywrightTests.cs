using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Renci.SshNet;
using Testcontainers.SqlEdge;

namespace Chirp.WebService.Tests.E2ETests;

public class PlaywrightTests : PageTest, IClassFixture<CustomWebApplicationFactory>
{
    private readonly SqlEdgeContainer _sqlEdgeContainer = new SqlEdgeBuilder().Build();
    
    public Task InitializeAsync()
    {
        return _sqlEdgeContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _sqlEdgeContainer.DisposeAsync().AsTask();
    }

    private readonly string _serverAddress;

    public PlaywrightTests(CustomWebApplicationFactory fixture)
    {
        _serverAddress = fixture.ServerAddress;
    }

    [Fact]
    public async Task ClickAuthorNameRedirects()
    {
        //Arrange
        using var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();
        
            //XPath for author name
            var authorXPath = "//*[@id=\"messagelist\"]/li[1]/p[1]/strong/a";
        
        await page.GotoAsync(_serverAddress);
    
        var authorButton = await page.QuerySelectorAsync(authorXPath);

        var authorName = await authorButton.InnerTextAsync();
            
        //Act
        await authorButton.ClickAsync();//Simulate click
        
        var headerElement = await page.WaitForSelectorAsync("//*[@id='userTimeline']");

        var headerElementValue = await headerElement.InnerTextAsync();

        Assert.Contains(authorName, headerElementValue);
    }


}