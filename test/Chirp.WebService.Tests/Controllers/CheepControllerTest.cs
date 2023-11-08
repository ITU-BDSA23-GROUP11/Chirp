using Chirp.Infrastructure.Tests.Repositories;
using Microsoft.Graph;
using Moq;

namespace Chirp.WebService.Tests.Controllers;

public class CheepControllerTest
{
    private static MockCheepRepository _mockRepository = MockRepositoryFactory.GetMockCheepRepository();

    [Fact]
    public void TestCreateCheep()
    {
       
    }
}