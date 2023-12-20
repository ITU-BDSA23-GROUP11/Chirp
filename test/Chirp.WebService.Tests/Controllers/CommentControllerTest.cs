using Bogus;
using Chirp.Core.Extensions;
using Chirp.Infrastructure.Models;
using Chirp.Tests.Core;
using Chirp.WebService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;

namespace Chirp.WebService.Tests.Controllers;

public class CommentControllerTest
{
    private readonly MockChirpRepositories _mockChirpRepositories = MockRepositoryFactory.GetMockCheepRepositories();
    private readonly CommentController _commentController;

    public CommentControllerTest()
    {
        Mock<CommentController> mockController = new(_mockChirpRepositories.AuthorRepository,
            _mockChirpRepositories.CheepRepository,
            _mockChirpRepositories.LikeRepository,
            _mockChirpRepositories.CommentRepository);
        mockController.CallBase = true;
        
        string name = new Faker().Name.FullName();
        var user = new ClaimsUser
        {
            Id = new Faker().Random.Guid(),
            Name = name,
            Username = new Faker().Internet.UserName(name),
            AvatarUrl = new Faker().Internet.Avatar()
        };
        mockController.As<IController>().Setup(bc => bc.GetUser).Returns(() => user);
        mockController.As<IController>().Setup(bc => bc.GetPathUrl).Returns(() => new Faker().Internet.UrlWithPath());

        _commentController = mockController.Object;
    }

    [Fact]
    public async Task TestAddComment()
    {
        //Arrange
        Author author = _mockChirpRepositories.TestAuthors.First();
        Cheep cheep = _mockChirpRepositories.TestCheeps.First();
        string cheepId = cheep.CheepId.ToString();
        string authorId = author.AuthorId.ToString();
        string commentText = new Faker().Random.Words();
        
        IFormCollection collection = new FormCollection(
            new Dictionary<string, StringValues>
            {
                { "CommentText", commentText },
                { "CheepId", cheepId },
                { "AuthorId", authorId }
            }
        );

        // Act
        var result = await _commentController.Add(collection);

        // Assert
        Assert.IsType<RedirectResult>(result);
        var redirectResult = result as RedirectResult;
        Assert.NotNull(redirectResult);
        Assert.False(string.IsNullOrEmpty(redirectResult.Url));
    }

    [Fact]
    public async Task TestDeleteComment()
    {
        //Arrange
        Comment comment = _mockChirpRepositories.TestComment.First();
        string commentId = comment.CommentId.ToString();

        IFormCollection collection = new FormCollection(
            new Dictionary<string, StringValues>
            {
                { "CommentId", commentId }
            }
        );
        
        // Act
        var result = await _commentController.Delete(collection);

        // Assert
        Assert.IsType<RedirectResult>(result);
        var redirectResult = result as RedirectResult;
        Assert.NotNull(redirectResult);
        Assert.False(string.IsNullOrEmpty(redirectResult.Url));
    }

   [Fact] 
    public async Task TestDeleteCommentFail()
    {
        //Arrange
        string fakeCommentId = "not_an_id";

        IFormCollection collection = new FormCollection(
            new Dictionary<string, StringValues>
            {
                { "CommentId", fakeCommentId }
            }
        );
        
        //Act
        IActionResult actionResult = await _commentController.Delete(collection);
        
        //Assert
        Assert.False(actionResult is RedirectResult);

    }
}