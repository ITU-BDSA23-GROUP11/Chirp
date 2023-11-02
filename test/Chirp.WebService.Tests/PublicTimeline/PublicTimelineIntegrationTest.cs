using Chirp.WebService.Tests.Utilities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Graph;
using Xunit.Abstractions;

namespace Chirp.WebService.Tests.PublicTimeline;

public class PublicTimelineIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient usableClient;
    private readonly ITestOutputHelper output;

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
    
    
    
}