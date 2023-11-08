using Chirp.Core.Dto;
using Chirp.Infrastructure.Tests.Repositories;
using Chirp.WebService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Chirp.WebService.Tests.Controllers;

public class CheepControllerTest
{
    private static MockCheepRepository _mockRepository = MockRepositoryFactory.GetMockCheepRepository();
    private static CheepController _cheepController = new CheepController(_mockRepository.CheepRepository);

    [Fact]
    public void TestCreateCheep()
    {
        //Arrange
        string uniqueContent = Guid.NewGuid().ToString();//Generate unique cheep content
        Dictionary<string, StringValues> formData = new Dictionary<String, StringValues>();
        formData.Add("cheepText", uniqueContent);
        
        IFormCollection collection = new FormCollection(
            formData,
            null
        );

        //Act
        ActionResult actionResult = _cheepController.Create(collection);
        
        //Assert
        List<CheepDto> newCheeps = _mockRepository.CheepRepository.GetCheepsForPage(1);
        
        //THIS CURRENTLY FAILS BECAUSE THE CHEEP IS NOT CREATED -> BAD REQUEST
        Assert.Equal(uniqueContent, newCheeps[0].Text);
    }
}