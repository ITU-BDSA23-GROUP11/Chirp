using Bogus;
using Chirp.Core.Dto;
using Chirp.Infrastructure.Models;
using Chirp.Tests.Core;
using Moq;

namespace Chirp.Infrastructure.Tests.Repositories;

public class AuthorRepositoryTest
{
    private readonly MockChirpRepositories _mockChirpRepositories = MockRepositoryFactory.GetMockCheepRepositories();
    
    [Fact]
    public void AddAuthorTest()
    {
        //Arrange
        Guid authorDtoId = Guid.NewGuid();
        string name = new Faker().Name.FullName();
        string username = new Faker().Internet.UserName(name);
        //Act
        _mockChirpRepositories.AuthorRepository.AddAuthor(new AuthorDto
        {
            Id = authorDtoId,
            Name = name,
            Username = username,
            AvatarUrl = new Faker().Internet.Avatar()
        });
        //Assert
        _mockChirpRepositories.MockAuthorsDbSet.Verify(m => m.Add(It.IsAny<Author>()), Times.Once);
        _mockChirpRepositories.MockChirpDbContext.Verify(m => m.SaveChanges(), Times.Once);
        
    }

    [Fact] //Tests if the follow is added to an author
    public void AddFollowTest()
    {
        //Arrange
        Author authorToBeFollowed = _mockChirpRepositories.TestAuthors.Last();
        Author followingAuthor = _mockChirpRepositories.TestAuthors.First();
        //Act
        _mockChirpRepositories.AuthorRepository.AddFollow(followingAuthor.AuthorId, authorToBeFollowed.AuthorId);
        //Assert
        Assert.Contains(authorToBeFollowed.FollowedBy,
            a => a.AuthorId.ToString().Equals(followingAuthor.AuthorId.ToString()));
    }

    [Fact]
    public void GetAuthorByEmailTest()
    {
        //Arrange
        Author author = _mockChirpRepositories.TestAuthors.First();
        //Act
        AuthorDto? resAuthor = _mockChirpRepositories.AuthorRepository.GetAuthorFromUsername(author.Username);
        //Assert
        Assert.Equal(author.AuthorId, resAuthor?.Id);
    }

    [Fact]
    public void GetFollowsForAuthorTest()
    {
        //Arrange
        Author authorToBeFollowed = _mockChirpRepositories.TestAuthors.Last();
        //Act
        List<string> followerList = _mockChirpRepositories.AuthorRepository.GetFollowsForAuthor(authorToBeFollowed.Username);
        //Assert
        Assert.NotEmpty(followerList);
    }

    [Fact]
    public void RemoveFollowTest()
    {
        //Arrange
        Author author = _mockChirpRepositories.TestAuthors.First();
        Author unfollowAuthor = author.Follows.First();
        //Act
        _mockChirpRepositories.AuthorRepository.RemoveFollow(author.AuthorId, unfollowAuthor.AuthorId);
        
        bool isFollowingRemoved = !author.Follows.Any(a => a.AuthorId == unfollowAuthor.AuthorId);
        bool isFollowedByRemoved = !unfollowAuthor.FollowedBy.Any(a => a.AuthorId == author.AuthorId);
        //Assert
        Assert.True(isFollowingRemoved);
        Assert.True(isFollowedByRemoved);
        _mockChirpRepositories.MockChirpDbContext.Verify(db => db.SaveChanges(), Times.Once);
    }
    
    
}

