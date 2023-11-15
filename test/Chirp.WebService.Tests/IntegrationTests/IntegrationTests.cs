using Microsoft.AspNetCore.Mvc.Testing;
using HtmlAgilityPack;
using Testcontainers.SqlEdge;
using Xunit.Abstractions;
using Xunit.Sdk;

// Used parts of https://github.com/testcontainers/testcontainers-dotnet/tree/develop/examples/WeatherForecast

namespace Chirp.WebService.Tests.IntegrationTests;

public sealed class IntegrationTests : IAsyncLifetime
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

    public sealed class RazorTest : IClassFixture<IntegrationTests>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _webApplicationFactory;

        private readonly IServiceScope _serviceScope;

        private readonly HttpClient _httpClient;
        
        public RazorTest(IntegrationTests webAppTest)
        {
            // Instead of using environment variables to bootstrap our application configuration, we can implement a custom WebApplicationFactory<TEntryPoint>
            // that overrides the ConfigureWebHost(IWebHostBuilder) method to add a WeatherDataContext to the service collection.
            Environment.SetEnvironmentVariable("ConnectionStrings__ChirpSqlDb", webAppTest._sqlEdgeContainer.GetConnectionString());
            _webApplicationFactory = new WebApplicationFactory<Program>();
            _serviceScope = _webApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            _httpClient = _webApplicationFactory.CreateClient();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _serviceScope.Dispose();
            _webApplicationFactory.Dispose();
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
            public async void FrontPageTheSameAsPage1()
            {
                //Act
                var frontPageRsp = await _httpClient.GetAsync("/");
                var page1Rsp = await _httpClient.GetAsync("/?page=1");
        
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
                var rsp = await _httpClient.GetAsync("/");
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
                var rsp = await _httpClient.GetAsync("/" + page);
                string htmlContent = await rsp.Content.ReadAsStringAsync();
                
                //Check that the page contains the parameter name
                Assert.Contains(page, htmlContent);
            }
    }
}