using Chirp.DBService.Contexts;
using Chirp.DBService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chirp.WebService;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public readonly IConfiguration Configuration;
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddScoped<ICheepRepository, CheepRepository>();
        services.AddDbContext<ChirpDbContext>(
            options =>
            {
                string rawConnectionString = (Configuration.GetConnectionString("ChirpDb") ?? "{TEMP_DIR}/chirp_db.db").Replace("{TEMP_DIR}", Path.GetTempPath());
                string connectionString = Path.DirectorySeparatorChar+Path.Join(rawConnectionString.Split("/"));
                options.UseSqlite($"Data Source={connectionString}");
                Console.WriteLine("ChirpDBContext database initialised at: "+connectionString);
            }
        );
    }

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

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
        });
    }
}