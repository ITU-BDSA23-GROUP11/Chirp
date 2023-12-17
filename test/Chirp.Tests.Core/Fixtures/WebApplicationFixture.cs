using Chirp.Tests.Core.Factories;
namespace Chirp.Tests.Core.Fixtures;

public class WebApplicationFixture : IDisposable
{
    public struct ClientAndFactory
    {
        public MockWebApplicationFactory Factory;
        public HttpClient Client;
    }
    
    public readonly List<MockWebApplicationFactory> Factories = new ();
    public readonly List<HttpClient> Clients = new ();

    public ClientAndFactory GetClient()
    {
        var newFactory = new MockWebApplicationFactory();
        Factories.Add(newFactory);
        
        var newClient = newFactory.CreateClient();
        Clients.Add(newClient);

        return new ClientAndFactory
        {
            Factory = newFactory,
            Client = newClient
        };
    }

    public void Dispose()
    {
        foreach (HttpClient httpClient in Clients)
        {
            httpClient.Dispose();
        }
        
        foreach (MockWebApplicationFactory mockWebApplicationFactory in Factories)
        {
            mockWebApplicationFactory.Dispose();
        }
    }
}