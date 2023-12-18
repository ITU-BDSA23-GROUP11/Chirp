using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Chirp.Tests.Core.Fixtures;
using Chirp.WebService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using Xunit;
using Program = Chirp.WebService.Program;

namespace Chirp.Tests.Core.Factories;

public class MockWebApplicationFactoryWithAuth : DbFixture, IAsyncLifetime, IDisposable
{
    private readonly IHost _host;
    private IPlaywright? Playwright { get; set; }
    public IBrowser? Browser { get; private set; }
    public string BaseUrl { get; } = $"http://localhost:{GetRandomUnusedPort()}";
    
    public MockWebApplicationFactoryWithAuth()
    {
        _host = Program
        .CreateHostBuilder(null!)
        .ConfigureWebHostDefaults(webBuilder => {
            webBuilder.UseStartup<Startup>();
            webBuilder.UseUrls(BaseUrl);
            // webBuilder.UseEnvironment("Development");
        })
        .ConfigureServices(services => {
            services
                .AddAuthentication(defaultScheme: "E2EScheme")
                .AddScheme<AuthenticationSchemeOptions, MockAuth>("E2EScheme",_ => {});
        })
        .Build();
    }

    public async Task InitializeAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync();
        await _host.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _host.StopAsync();
        _host.Dispose();
        Playwright?.Dispose();
    }

    public new void Dispose()
    {
        _host.Dispose();
        Playwright?.Dispose();
    }

    private static int GetRandomUnusedPort()
    {
        var listener = new TcpListener(IPAddress.Any, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}

public class MockAuth : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public MockAuth(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock) { }
    
    //Used to simulate an authenticated user
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { 
            new Claim("name", "PlaywrightTester"), 
            new Claim("avatar_url", "https://avatars.githubusercontent.com/u/118434623?v=4"),
            new Claim("login", "E2ETEST"),
            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "6c61076a-6dba-4f8f-b7dd-ac9e3f0fb029")
        };
        
        var identity = new ClaimsIdentity(claims, "E2ETest");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "E2EScheme");

        var result = AuthenticateResult.Success(ticket);
        
        return Task.FromResult(result);
    }
}