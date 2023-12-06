using Bogus;
using Chirp.Core.Dto;
using Chirp.Infrastructure.Models;
using Chirp.Tests.Core;
using Moq;
using Org.BouncyCastle.Asn1.Cms;

namespace Chirp.Infrastructure.Tests.Repositories;

public class AuthorRepositoryTest
{
    private readonly MockChirpRepositories _mockChirpRepositories = MockRepositoryFactory.GetMockCheepRepository();
    
    [Fact]
    public void AddAuthorTest()
    {
        //Arrange
        Guid authorDtoId = Guid.NewGuid();
        string name = new Faker().Random.Words();
        string authorEmail = "testing@mail.com";
        //Act
        _mockChirpRepositories.AuthorRepository.AddAuthor(new AuthorDto
        {
            Email = authorEmail,
            Name = name,
            Id = authorDtoId
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
        _mockChirpRepositories.AuthorRepository.AddFollow(authorToBeFollowed.Email, followingAuthor.Email);
        //Assert
        string actual = followingAuthor.FollowedBy.First().Email;
        Assert.Equal(authorToBeFollowed.Email, actual);
    }

    [Fact]
    public void GetAuthorByEmailTest()
    {
        //Arrange
        Author author = _mockChirpRepositories.TestAuthors.First();
        string authorEmail = author.Email;
        //Act
        string name = _mockChirpRepositories.AuthorRepository.GetAuthorNameByEmail(authorEmail);
        //Assert
        Assert.Equal(name, author.Name);
    }
    
    
}

