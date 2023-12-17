using Chirp.Core.Repositories;
using Microsoft.EntityFrameworkCore;
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
        
        webApplication.Run();
    }

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
        services.AddHttpClient();
        services
            .AddRazorPages(options =>
            {
                options.Conventions.AllowAnonymousToPage("/Index");
            })
            .AddMvcOptions(_ => { })
            .AddMicrosoftIdentityUI();
        
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<ICheepRepository, CheepRepository>();
        services.AddScoped<ILikeRepository, LikeRepository>();
        services.AddSingleton(configuration);
        
        var sqlConnectionString = new SqlConnectionStringBuilder(configuration.GetConnectionString("ChirpSqlDb"));
        string? password = configuration["DB:Password"];
        
        if (string.IsNullOrEmpty(sqlConnectionString.Password) && password != null)
        {
            // Add local password
            sqlConnectionString.Password = password;
        } 
        
        services.AddDbContext<ChirpDbContext>(options =>
            options.UseSqlServer(sqlConnectionString.ConnectionString));
      
        
    }
    
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
}