using Testcontainers.SqlEdge;

namespace Chirp.Tests.Core.Fixtures;

public class DbFixture : IDisposable
{
    private readonly SqlEdgeContainer _container;

    protected DbFixture()
    {
        _container = new SqlEdgeBuilder()
            .WithImage("mcr.microsoft.com/azure-sql-edge")
            .Build();
        _container.StartAsync().Wait();
        
        Environment.SetEnvironmentVariable("ConnectionStrings__ChirpSqlDb", _container.GetConnectionString());
    }
    
    public void Dispose()
    {
        _container.DisposeAsync().AsTask().Wait();
    }
}