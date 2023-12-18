using Bogus;
using Chirp.Core.Extensions;
using Chirp.Infrastructure.Models;
using Chirp.Tests.Core;
using Chirp.WebService.Controllers;
using Microsoft.Extensions.Primitives;
using Moq;

namespace Chirp.WebService.Tests.Controllers;

public class CommentControllerTest
{
    private readonly MockChirpRepositories _mockChirpRepositories = MockRepositoryFactory.GetMockCheepRepositories();
    private readonly CommentController _commentController;
    private readonly Mock<CommentController> _mockController;

    public CommentControllerTest()
    {
        _mockController = new Mock<CommentController>(_mockChirpRepositories.AuthorRepository,
            _mockChirpRepositories.CheepRepository,
            _mockChirpRepositories.LikeRepository,
            _mockChirpRepositories.CommentRepository);
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

        _commentController = _mockController.Object;
        

    }

    [Fact]
    public void TestAddComment()
    {
        Author author = _mockChirpRepositories.TestAuthors.First();
        Cheep cheep = _mockChirpRepositories.TestCheeps.First();
      
    }
}