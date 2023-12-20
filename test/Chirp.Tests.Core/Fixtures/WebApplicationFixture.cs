using Chirp.Tests.Core.Factories;
namespace Chirp.Tests.Core.Fixtures;

public class WebApplicationFixture : IDisposable
{
    private readonly List<MockWebApplicationFactory> _factories = new ();
    private readonly List<HttpClient> _clients = new ();

    public HttpClient GetClient()
    {
        var newFactory = new MockWebApplicationFactory();
        _factories.Add(newFactory);
        
        var newClient = newFactory.CreateClient();
        _clients.Add(newClient);

        return newClient;
    }

    public void Dispose()
    {
        foreach (HttpClient httpClient in _clients)
        {
            httpClient.Dispose();
        }
        
        foreach (MockWebApplicationFactory mockWebApplicationFactory in _factories)
        {
            mockWebApplicationFactory.Dispose();
        }
    }
}