using Testcontainers.SqlEdge;

namespace Chirp.Tests.Core.Fixtures;

public class DbFixture : IDisposable
{
    public readonly SqlEdgeContainer Container;
    
    public DbFixture()
    {
        Container = new SqlEdgeBuilder()
            .WithImage("mcr.microsoft.com/azure-sql-edge")
            .Build();
        Container.StartAsync().Wait();
        
        Environment.SetEnvironmentVariable("ConnectionStrings__ChirpSqlDb", Container.GetConnectionString());
    }
    
    public void Dispose()
    {
        Container.DisposeAsync().AsTask().Wait();
    }
}