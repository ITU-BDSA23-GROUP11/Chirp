using Chirp.Infrastructure.Contexts;
using Microsoft.AspNetCore.Mvc.Testing;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Testcontainers.SqlEdge;

namespace Chirp.WebService.Tests.IntegrationTests;

public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _usableClient;

    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        // Start up test container
        var container = new SqlEdgeBuilder()
            .WithImage("mcr.microsoft.com/azure-sql-edge")
            .Build();
        
        container.StartAsync().Wait();

        // Set up services
        factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContext = services.SingleOrDefault(
                        d => d.ServiceType ==
                             typeof(DbContextOptions<ChirpDbContext>));

                    if (dbContext != null) services.Remove(dbContext);

                    services.AddDbContext<ChirpDbContext>(opts =>
                    {
                        opts.UseSqlServer(container.GetConnectionString());
                    });
                });
            });
        
        // Ensure DB is created
        using (var scope = factory.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                var chirpDbContext = services.GetRequiredService<ChirpDbContext>();
                chirpDbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating the DB.");
            }
        }
        
        // Build client
        _usableClient = factory
            .CreateClient(new WebApplicationFactoryClientOptions
                { AllowAutoRedirect = true, HandleCookies = true });
    }

    [Fact]  
    public async void CanEstablishConnection()
    {
        //Act
        var rsp = await _usableClient.GetAsync("/");
        rsp.EnsureSuccessStatusCode();
        
        //Assert
        Assert.Equal("OK", rsp.StatusCode.ToString());
    }

    [Fact]
    public async void FrontPageTheSameAsPage1()
    {
        //Act
        var frontPageRsp = await _usableClient.GetAsync("/");
        var page1Rsp = await _usableClient.GetAsync("/?page=1");

        string frontPageContent = await frontPageRsp.Content.ReadAsStringAsync();
        string page1RspContent = await page1Rsp.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", frontPageContent);
        Assert.Contains("Chirp!", page1RspContent);
        Assert.Equal(frontPageContent, page1RspContent);
    }
    
    [Fact]
    public async void FrontPageContains32Cheeps()
    {
        //Arrange & Act
        var rsp = await _usableClient.GetAsync("/");
        string htmlContent = await rsp.Content.ReadAsStringAsync();

        //Parse the htmlContent to a HTMLDocument
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);

        int amountOfListItems = doc.DocumentNode.SelectNodes("//li").Count();

        Assert.Equal(32, amountOfListItems);
    }

    [Theory]
    [InlineData("Helge")]
    [InlineData("SampleUser")]
    public async void PrivateTimelinesAreDisplayed(String page)
    {
        //Act
        var rsp = await _usableClient.GetAsync("/" + page);
        string htmlContent = await rsp.Content.ReadAsStringAsync();
        
        //Check that the page contains the parameter name
        Assert.Contains(page, htmlContent);
    }
}