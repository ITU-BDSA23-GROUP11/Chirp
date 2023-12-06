using System.Security.Claims;
using System.Text.Encodings.Web;
using Chirp.Infrastructure.Contexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
namespace Chirp.WebService.Tests.E2ETests;

public class WebApplicationFactoryWithAuth<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(s =>
        {
            //Remove the default DBContext configuration
            var descriptor = s.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ChirpDbContext>));

            if (descriptor != null)
            {
                s.Remove(descriptor);
            }
            
            //Create an in-memory DB instance
            s.AddDbContext<ChirpDbContext>(options =>
            {
                options.UseInMemoryDatabase("MemoryDB");
            });
            
            s.AddAuthentication(defaultScheme: "E2EScheme")
                .AddScheme<AuthenticationSchemeOptions, MockAuth>(
                    "E2EScheme",options => {});
        });

        builder.UseEnvironment("Development");
    }
}

public class MockAuth : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public MockAuth(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { 
            new Claim(ClaimTypes.Name, "PlaywrightTester"), 
            new Claim(ClaimTypes.Email, "bdsagrup11@gmail.com"), 
            new Claim(ClaimTypes.GivenName, "E2E"), 
            new Claim(ClaimTypes.Surname, "User") 
        };
        var identity = new ClaimsIdentity(claims, "E2ETest");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "E2EScheme");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}