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
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseStartup<Startup>();
            })
            .ConfigureHostConfiguration(builder =>
            {
                builder.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                ConfigureServices(context.Configuration, services);
            });

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
        services.AddScoped<ICommentRepository, CommentRepository>();
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
}

public class Startup
{
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
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
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
        });
    }
}