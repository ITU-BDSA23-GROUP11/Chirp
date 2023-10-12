using Chirp.DBService.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Chirp.WebService;

// Used https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/data/ef-rp/intro/samples/cu50 as inspiration

public class Program
{
    public static void Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(builder =>
        {
            builder.UseStartup<Startup>();
        }).Build();
        
        BootstrapDb(host);
        
        host.Run();
    }

    private static void BootstrapDb(IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                var chirpDbContext = services.GetRequiredService<ChirpDbContext>();
                // chirpDbContext.Database.EnsureCreated();
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