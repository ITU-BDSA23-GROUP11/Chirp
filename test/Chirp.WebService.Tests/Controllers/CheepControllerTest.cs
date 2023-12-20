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
        _mockController = new Mock<CheepController>(_mockChirpRepositories.AuthorRepository,
            _mockChirpRepositories.CheepRepository, _mockChirpRepositories.LikeRepository, _mockChirpRepositories.CommentRepository);
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
    public void TestFollowReturnsRedirect()
    {
        Author author = _mockChirpRepositories.TestAuthors.First();

        IFormCollection collection = new FormCollection(
            new Dictionary<string, StringValues>
            {
                {"CheepAuthorId", author.AuthorId.ToString()}
            }
        );
        IActionResult actionResult = _cheepController.Follow(collection);
        
        Assert.True(actionResult is RedirectResult);
    }

    [Fact]
    public void TestUnfollowReturnsRedirect()
    {
        Author author = _mockChirpRepositories.TestAuthors.First().Follows.First();

        IFormCollection collection = new FormCollection(
            new Dictionary<string, StringValues>
            {
                {"CheepAuthorId", author.AuthorId.ToString()}
            }
        );

        IActionResult actionResult = _cheepController.Unfollow(collection);
        Assert.True(actionResult is RedirectResult);
    }
    
    [Fact]
    public async Task TestCreateReturnsRedirect()
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
        IActionResult actionResult = await _cheepController.Create(collection);

        
        //Assert
        Assert.True(actionResult is RedirectResult);
    }

    [Fact]
    public async Task TestCreateReturnsUnauthorized()
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
        IActionResult actionResult = await _cheepController.Create(collection);
        
        //Assert
        Assert.True(actionResult is UnauthorizedResult);
    }

    [Fact]
    public async Task TestCreateEmptyCheepReturnsBad()
    {
        //Arrange
        IFormCollection collection = new FormCollection(
            new Dictionary<string, StringValues>
            {
                { "cheepText", ""}
            }
        );
        
        //Act
        IActionResult actionResult = await _cheepController.Create(collection);
        
        //Assert
        Assert.True(actionResult is RedirectResult);
        var redirectUrl = ((RedirectResult)actionResult).Url;
        Assert.Contains("errorMessage=Invalid input", redirectUrl);
    }

    [Fact]
    public async Task TestCreateTooLongCheepReturnsBad()
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
        IActionResult actionResult = await _cheepController.Create(collection);
        
        //Assert
        Assert.True(actionResult is RedirectResult);
        var redirectUrl = ((RedirectResult)actionResult).Url;
        Assert.Contains("errorMessage=Invalid input - cheep is too long (max 160 characters)", redirectUrl);
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
        Assert.True(actionResult is RedirectResult);
        var redirectUrl = ((RedirectResult)actionResult).Url;
        Assert.Contains("errorMessage=Invalid input", redirectUrl);
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
    public async Task DeleteCheepReturnsRedirectTest()
    {
        Cheep cheep = _mockChirpRepositories.TestCheeps.First();
        string cheepId = cheep.CheepId.ToString();

        IFormCollection collection = new FormCollection(
            new Dictionary<string, StringValues>
            {
                { "cheepId", cheepId}
            }
        );
        IActionResult actionResult = await _cheepController.Delete(collection);
        Assert.True(actionResult is RedirectResult);
    }


    
}
