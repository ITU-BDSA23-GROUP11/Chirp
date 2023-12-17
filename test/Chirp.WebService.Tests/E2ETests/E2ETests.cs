using Chirp.Infrastructure.Models;
using Chirp.Tests.Core;
using Moq;

namespace Chirp.WebService.Tests.E2ETests;

public class E2ETests : IClassFixture<MockWebApplicationFactoryWithAuth>
{
    private readonly MockWebApplicationFactoryWithAuth _fixture;
    
    public E2ETests(MockWebApplicationFactoryWithAuth fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task ClickAuthorNameRedirects()
    {
        //Arrange
        // using var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        // await using var browser = await playwright.Chromium.LaunchAsync();
        // var page = await browser.NewPageAsync();
        var page = await _fixture
            .Browser
            .NewPageAsync();
        await page.GotoAsync(_fixture.BaseUrl);
        
        //XPath for author name
        var authorXPath = "//*[@id='messagelist']/li[1]/p[1]/strong/a";
    
        var authorButton = await page.QuerySelectorAsync(authorXPath);
    
        if (authorButton == null) Assert.Fail();
    
        var authorName = await authorButton.InnerTextAsync();
            
        //Act
        await authorButton.ClickAsync();//Simulate click
        
        var headerElement = await page.WaitForSelectorAsync("//*[@id='userTimeline']");
    
        if (headerElement == null) Assert.Fail();
        
        var headerElementValue = await headerElement.InnerTextAsync();
    
        Assert.Contains(authorName, headerElementValue);
    }
    
    [Fact]
    public async Task CanDeleteCheep()
    {
        //Arrange
        // using var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        // await using var browser = await playwright.Chromium.LaunchAsync();
        // var page = await browser.NewPageAsync();
        // await page.GotoAsync(fixture.BaseUrl);
        var page = await _fixture.Browser.NewPageAsync();
        await page.GotoAsync(_fixture.BaseUrl);

        string cheepMessage = "This is a cheep deleted by the E2E test!";

        var shareButton = await page.QuerySelectorAsync("//*[@class='cheepbox']/form/input[2]");
        
        if (shareButton == null) Assert.Fail();

        //Act
        await page.GetByPlaceholder("This is a cheep...").FillAsync(cheepMessage);
        await shareButton.ClickAsync();

        //Status: the first cheep should now have been created
        var cheepDeleteButton = await page.QuerySelectorAsync("//*[@id='messagelist']/li[1]/form/button");

        if (cheepDeleteButton == null) Assert.Fail();
        
        await cheepDeleteButton.ClickAsync();
        
        //Status: the first cheep should now have been deleted
        
        var firstCheepLocation = await page.QuerySelectorAsync("//*[@id='messagelist']/li[1]/p[1]");

        if (firstCheepLocation == null) Assert.Fail();
        
        var firstCheepText = await firstCheepLocation.InnerTextAsync();

        Assert.DoesNotContain(cheepMessage, firstCheepText);
    }
    
    [Fact]
    public async Task CanCreateCheep()
    {
        //Arrange
        // using var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        // await using var browser = await playwright.Chromium.LaunchAsync();
        // var page = await browser.NewPageAsync();
        //
        var page = await _fixture.Browser.NewPageAsync();
        await page.GotoAsync(_fixture.BaseUrl);
    
        string cheepMessage = "This is a cheep generated by the E2E test!";
        
        var shareButton = await page.QuerySelectorAsync("//*[@class='cheepbox']/form/input[2]");
    
        if (shareButton == null) Assert.Fail();
    
        //Act
        await page.GetByPlaceholder("This is a cheep...").FillAsync(cheepMessage);
        await shareButton.ClickAsync();
    
        //Status: the first cheep should now have been created
    
        var firstCheepLocation = await page.QuerySelectorAsync("//*[@id='messagelist']/li[1]/p[1]");
    
        if (firstCheepLocation == null) Assert.Fail();
        
        var firstCheepText = await firstCheepLocation.InnerTextAsync();
    
        Assert.Contains(cheepMessage, firstCheepText);
    }
}