using Bogus;
using Chirp.Core.Dto;
using Chirp.Infrastructure.Tests.Repositories;
using Chirp.WebService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;

namespace Chirp.WebService.Tests.Controllers;

public class CheepControllerTest
{
    private readonly MockCheepRepository _mockCheepRepository = MockRepositoryFactory.GetMockCheepRepository();
    private readonly CheepController _cheepController;
    private Mock<CheepController> mockController;
    
    public CheepControllerTest()
    {
        mockController = new Mock<CheepController>(_mockCheepRepository.CheepRepository);
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
    public void TestCreateReturnsRedirect()
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
        Assert.True(actionResult is RedirectResult);
    }

    [Fact]
    public void TestCreateReturnsUnauthorized()
    {
        //Arrange
        //Simulate a non-authenticated user
        mockController.As<IController>().Setup(bc => bc.IsUserAuthenticated).Returns(() => false);

        String newCheep = new Faker().Random.Words();

        IFormCollection collection = new FormCollection(
            new Dictionary<string, StringValues>
            {
                { "cheepText", newCheep }
            }
        );
        
        //Act
        ActionResult actionResult = _cheepController.Create(collection);
        
        //Assert
        Assert.True(actionResult is UnauthorizedResult);
    }

    [Fact]
    public void TestCreateEmptyCheepReturnsBad()
    {
        //Arrange
        IFormCollection collection = new FormCollection(
            new Dictionary<string, StringValues>
            {
                { "cheepText", ""}
            }
        );
        
        //Act
        ActionResult actionResult = _cheepController.Create(collection);
        
        //Assert
        Assert.True(actionResult is BadRequestObjectResult);
    }

    [Fact]
    public void TestCreateTooLongCheepReturnsBad()
    {
        //Arrange
        string newCheep = new Faker().Random.String(161, 200);

        IFormCollection collection = new FormCollection(
            new Dictionary<string, StringValues>
            {
                {"cheepText", newCheep}
            }
        );
        
        //Act
        ActionResult actionResult = _cheepController.Create(collection);
        
        //Assert
        Assert.True(actionResult is UnauthorizedResult);
    }
}