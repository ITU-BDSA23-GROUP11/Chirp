using System.Security.Claims;
using System.Text.Encodings.Web;
using Chirp.Core.Repositories;
using Chirp.WebService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chirp.Tests.Core;

public class MockWebApplicationFactory : WebApplicationFactory<Program>
{
    private IHost? _host;

    public string ServerAddress => ClientOptions.BaseAddress.ToString();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(servicesConfiguration =>
        {
            var mockRepositories = MockRepositoryFactory.GetMockCheepRepositories();
            
            servicesConfiguration.AddScoped<ICheepRepository>(di => mockRepositories.CheepRepository);
            servicesConfiguration.AddScoped<IAuthorRepository>(di => mockRepositories.AuthorRepository);
            servicesConfiguration.AddScoped<ILikeRepository>(di => mockRepositories.LikeRepository);
            
            
            servicesConfiguration.AddAuthentication(defaultScheme: "E2EScheme")
                .AddScheme<AuthenticationSchemeOptions, MockAuth>(
                    "E2EScheme",options => {});
        });
        
        builder.UseEnvironment("Development");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Create the host for TestServer now before we
        // modify the builder to use Kestrel instead.
        var testHost = builder.Build();

        // Modify the host builder to use Kestrel instead
        // of TestServer so we can listen on a real address.
        
        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());
        
        // Create and start the Kestrel server before the test server,
        // otherwise due to the way the deferred host builder works
        // for minimal hosting, the server will not get "initialized
        // enough" for the address it is listening on to be available.
        // See https://github.com/dotnet/aspnetcore/issues/33846.
        _host = builder.Build();
        _host.Start();

        // Extract the selected dynamic port out of the Kestrel server
        // and assign it onto the client options for convenience so it
        // "just works" as otherwise it'll be the default http://localhost
        // URL, which won't route to the Kestrel-hosted HTTP server.
         var server = _host.Services.GetRequiredService<IServer>();
         var addresses = server.Features.Get<IServerAddressesFeature>();

        ClientOptions.BaseAddress = addresses!.Addresses
            .Select(x => new Uri(x))
            .Last();

        testHost.Start();
        return testHost;
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