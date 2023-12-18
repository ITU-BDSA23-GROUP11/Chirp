using Chirp.Core.Repositories;
using Chirp.WebService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Chirp.Tests.Core.Factories;

public class MockWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(servicesConfiguration =>
        {
            var mockRepositories = MockRepositoryFactory.GetMockCheepRepositories();
            
            servicesConfiguration.AddScoped<ICheepRepository>(_ => mockRepositories.CheepRepository);
            servicesConfiguration.AddScoped<IAuthorRepository>(_ => mockRepositories.AuthorRepository);
            servicesConfiguration.AddScoped<ILikeRepository>(_ => mockRepositories.LikeRepository);
        });
        
        builder.UseEnvironment("Development");
    }
}