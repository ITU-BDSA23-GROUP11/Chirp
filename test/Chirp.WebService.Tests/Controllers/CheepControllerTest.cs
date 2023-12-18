using Bogus;
using Chirp.Core.Extensions;
using Chirp.Infrastructure.Models;
using Chirp.Tests.Core;
using Chirp.WebService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;

namespace Chirp.WebService.Tests.Controllers;

public class CheepControllerTest
{
    private readonly MockChirpRepositories _mockChirpRepositories = MockRepositoryFactory.GetMockCheepRepositories();
    private readonly CheepController _cheepController;
    private readonly Mock<CheepController> _mockController;
    
    public CheepControllerTest()
    {
        _mockController = new Mock<CheepController>(_mockChirpRepositories.AuthorRepository, _mockChirpRepositories.CheepRepository, _mockChirpRepositories.LikeRepository);
        _mockController.CallBase = true;
            
        string name = new Faker().Name.FullName();
        var user = new ClaimsUser
        {
            Id = new Faker().Random.Guid(),
            Name = name,
            Username = new Faker().Internet.UserName(name),
            AvatarUrl = new Faker().Internet.Avatar()
        };
        _mockController.As<IController>().Setup(bc => bc.GetUser).Returns(() => user);
        _mockController.As<IController>().Setup(bc => bc.GetPathUrl).Returns(() => new Faker().Internet.UrlWithPath());

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
        _mockController.As<IController>().Setup(bc => bc.GetUser).Returns(() => () =>null);

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

    [Fact] //Tests if a cheep is not able to be liked
    public void LikeCheepReturnsBadTest()
    {
        //Arrange
        string cheepId = new Faker().Random.String(0);
        //Act
        IFormCollection collection = new FormCollection(
            new Dictionary<string, StringValues>
            {
                { "cheepdId", cheepId }
            }
        );
        //Assert
        IActionResult actionResult = _cheepController.Like(collection);
        Assert.True(actionResult is BadRequestObjectResult);

    }
    
    [Fact]
    public void LikeCheepReturnsRedirectTest()
    {
        //Arrange
        Cheep cheep = _mockChirpRepositories.TestCheeps.First();
        string cheepId = cheep.CheepId.ToString();
        IFormCollection collection = new FormCollection(
            new Dictionary<string, StringValues>
            {
                {"cheepId", cheepId}

            }
        );

        //Act
        IActionResult actionResult = _cheepController.Like(collection);
        //Assert
        Assert.True(actionResult is RedirectResult);
    }

    [Fact] //Like is successfully removed
    public void UnlikeCheepReturnsRedirectTest()
    {
        Like like = _mockChirpRepositories.TestLikes.First();
        string cheepId = like.Cheep.CheepId.ToString();

        IFormCollection collection = new FormCollection(
            new Dictionary<string, StringValues>
            {
                { "cheepId", cheepId }
            }
        );

        IActionResult actionResult = _cheepController.Unlike(collection);
        Assert.True(actionResult is RedirectResult);
    }

    [Fact]
    public void DeleteCheepReturnsRedirectTest()
    {
        Cheep cheep = _mockChirpRepositories.TestCheeps.First();
        string cheepId = cheep.CheepId.ToString();

        IFormCollection collection = new FormCollection(
            new Dictionary<string, StringValues>
            {
                { "cheepId", cheepId}
            }
        );
        IActionResult actionResult = _cheepController.Delete(collection);
        Assert.True(actionResult is RedirectResult);
    }


    
}
