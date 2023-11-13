using System.Diagnostics.CodeAnalysis;
using Chirp.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Contexts;
using Chirp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace Chirp.WebService;

// Used https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/data/ef-rp/intro/samples/cu50 as inspiration

public class Program
{
    public static void Main(string[] args)
    {
        var webApplicationBuilder = WebApplication.CreateBuilder(args);
        webApplicationBuilder.Configuration.AddEnvironmentVariables();
        ConfigureServices(webApplicationBuilder.Configuration, webApplicationBuilder.Services);

        var webApplication = webApplicationBuilder.Build();
        
        ConfigureMiddleware(webApplication);
        
        BootstrapDb(webApplication);
        
        webApplication.Run();
    }

    [ExcludeFromCodeCoverage]
    private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(configuration.GetSection("AzureADB2C"));
        services.AddAuthorization(options =>
        {
            // By default, all incoming requests will be authorized according to 
            // the default policy
            options.FallbackPolicy = options.DefaultPolicy;
        });
        services
            .AddRazorPages(options => {
                options.Conventions.AllowAnonymousToPage("/Index");
            })
            .AddMvcOptions(_ => { })
            .AddMicrosoftIdentityUI();
        services.AddScoped<ICheepRepository, CheepRepository>();
        services.AddSingleton(configuration);
        services.AddDbContext<ChirpDbContext>(options =>
        {
            SqlConnectionStringBuilder sqlConnectionString = new SqlConnectionStringBuilder(configuration.GetConnectionString("ChirpSqlDb"));
            string? password = configuration["DB:Password"];
            if (password != null)
            {
                sqlConnectionString.Password = password;
            }
            options.UseSqlServer(sqlConnectionString.ConnectionString);
        });
    }
    
    [ExcludeFromCodeCoverage]
    private static void ConfigureMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllers();
    }
    
    [ExcludeFromCodeCoverage]
    private static void BootstrapDb(IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                var chirpDbContext = services.GetRequiredService<ChirpDbContext>();
                chirpDbContext.Database.Migrate();
                DbInitializer.SeedDatabase(chirpDbContext);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating the DB.");
            }
        }
    }
}