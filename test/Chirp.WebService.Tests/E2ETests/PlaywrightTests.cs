using Testcontainers.SqlEdge;

namespace Chirp.WebService.Tests.E2ETests;

public class PlaywrightTests
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
        
    }


}