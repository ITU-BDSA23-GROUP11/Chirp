using Bogus;
using Chirp.Tests.Core;
using Chirp.WebService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;

namespace Chirp.WebService.Tests.Controllers;

public class CheepControllerTest
{
    private readonly MockChirpRepositories _mockChirpRepositories = MockRepositoryFactory.GetMockCheepRepository();
    private readonly CheepController _cheepController;
    private readonly Mock<CheepController> _mockController;
    
    public CheepControllerTest()
    {
        _mockController = new Mock<CheepController>(_mockChirpRepositories.AuthorRepository, _mockChirpRepositories.CheepRepository, _mockChirpRepositories.LikeRepository);
        _mockController.CallBase = true;
        _mockController.As<IController>().Setup(bc => bc.IsUserAuthenticated).Returns(() => true);
            
        string firstName = new Faker().Name.FirstName();
        string lastName = new Faker().Name.LastName();
        _mockController.As<IController>().Setup(bc => bc.GetUserFullName).Returns(() => $"{firstName} {lastName}");
        _mockController.As<IController>().Setup(bc => bc.GetUserEmail).Returns(() => new Faker().Internet.Email(firstName,lastName));
        _mockController.As<IController>().Setup(bc => bc.GetUserEmail).Returns(() => new Faker().Internet.Email(firstName,lastName));
        _mockController.As<IController>().Setup(bc => bc.GetPathUrl).Returns(() => new Faker().Internet.UrlWithPath());
        _mockController.As<IController>().Setup(bc => bc.GetUserId).Returns(() => new Faker().Random.Guid());

        _cheepController = _mockController.Object;
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
        _mockController.As<IController>().Setup(bc => bc.IsUserAuthenticated).Returns(() => false);

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
        Assert.True(actionResult is BadRequestObjectResult);
    }

}
