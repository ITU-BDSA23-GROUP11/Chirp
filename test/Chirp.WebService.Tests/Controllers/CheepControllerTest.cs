using Bogus;
using Chirp.Core.Dto;
using Chirp.Infrastructure.Tests.Repositories;
using Chirp.WebService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Graph;
using Moq;

namespace Chirp.WebService.Tests.Controllers;

public class CheepControllerTest
{
    private readonly MockCheepRepository _mockCheepRepository = MockRepositoryFactory.GetMockCheepRepository();
    private readonly CheepController _cheepController;
    
    public CheepControllerTest()
    {
        var mockController = new Mock<CheepController>(_mockCheepRepository.CheepRepository);
        mockController.CallBase = true;
        mockController.As<IController>().Setup(bc => bc.IsUserAuthenticated).Returns(() => true);
            
        string firstName = new Faker().Name.FirstName();
        string lastName = new Faker().Name.LastName();
        mockController.As<IController>().Setup(bc => bc.GetUserFullName).Returns(() => $"{firstName} {lastName}");
        mockController.As<IController>().Setup(bc => bc.GetUserEmail).Returns(() => new Faker().Internet.Email(firstName,lastName));
        mockController.As<IController>().Setup(bc => bc.GetPathUrl).Returns(() => new Faker().Internet.UrlWithPath());

        _cheepController = mockController.Object;
    }
    
    [Fact]
    public void TestCreateCheep()
    {
        //Arrange
        string newCheepText = new Faker().Random.Words(); //Generate unique/random cheep content
        
        IFormCollection collection = new FormCollection(
            new Dictionary<string, StringValues>
            {
                {"cheepText", newCheepText}
            }
        );

        //Act
        ActionResult actionResult = _cheepController.Create(collection);
        
        //Assert
        List<CheepDto> newCheeps = _mockCheepRepository.CheepRepository.GetCheepsForPage(0);
        
        //THIS CURRENTLY FAILS BECAUSE THE CHEEP IS NOT CREATED -> BAD REQUEST
        Assert.Equal(newCheepText, newCheeps[0].Text);
    }

    [Fact]
    public void TestDeleteCheep_BadRequest()
    {
        // Arrange
        var formCollection = new FormCollection(new Dictionary<string, StringValues>());

        //Act
        var result = _cheepController.Delete(formCollection);

        //Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

}