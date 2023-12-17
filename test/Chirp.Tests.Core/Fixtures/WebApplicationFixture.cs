namespace Chirp.Tests.Core.Fixtures;

public class WebApplicationFixture : IDisposable
{
    public readonly MockWebApplicationFactory MockWebApplicationFactory = new ();
    public readonly List<HttpClient> Clients = new ();

    public HttpClient GetClient()
    {
        var newClient = MockWebApplicationFactory.CreateClient();
        Clients.Add(newClient);
        return newClient;
    }

    public void Dispose()
    {
        foreach (HttpClient httpClient in Clients)
        {
            httpClient.Dispose();
        }
        
        MockWebApplicationFactory.Dispose();
    }
}