using Bogus;
using Chirp.Core.Dto;
using Chirp.Infrastructure.Models;
using Chirp.Tests.Core;
using Moq;

namespace Chirp.Infrastructure.Tests.Repositories;

public class AuthorRepositoryTest
{
    private readonly MockChirpRepositories _mockChirpRepositories = MockRepositoryFactory.GetMockCheepRepository();
    
    [Fact]
    public void AddAuthorTest()
    {
        //Arrange
        Guid authorDtoId = Guid.NewGuid();
        string name = new Faker().Name.FullName();
        string login = new Faker().Internet.UserName(name);
        //Act
        _mockChirpRepositories.AuthorRepository.AddAuthor(new AuthorDto
        {
            Id = authorDtoId,
            Name = name,
            Login = login,
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
        AuthorDto? resAuthor = _mockChirpRepositories.AuthorRepository.GetAuthorFromLogin(author.Login);
        //Assert
        Assert.Equal(author.AuthorId, resAuthor?.Id);
    }
}

