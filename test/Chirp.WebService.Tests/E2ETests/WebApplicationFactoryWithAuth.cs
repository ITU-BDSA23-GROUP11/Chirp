using System.Security.Claims;
using System.Text.Encodings.Web;
using Chirp.Infrastructure;
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
            
            using (var scope = s.BuildServiceProvider().CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ChirpDbContext>();
                dbContext.Database.EnsureCreated();

                DbInitializer.SeedDatabase(dbContext);
            }
            
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
        string userId = Guid.NewGuid().ToString();
        
        var claims = new[] { 
            //ID Claim for user
            new Claim(ClaimTypes.Name, "PlaywrightTester"), 
            new Claim(ClaimTypes.Email, "bdsagrup11@gmail.com"), 
            new Claim(ClaimTypes.GivenName, "E2E"), 
            new Claim(ClaimTypes.Surname, "User"),
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "a93a2972-bc51-4a82-ad98-bfb8bd07434f")
        };
        
        var identity = new ClaimsIdentity(claims, "E2ETest");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "E2EScheme");

        var result = AuthenticateResult.Success(ticket);

        Console.WriteLine("break for investigation");

        return Task.FromResult(result);
    }
}