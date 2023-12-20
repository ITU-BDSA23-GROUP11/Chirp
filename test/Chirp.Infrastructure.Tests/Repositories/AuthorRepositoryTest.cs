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
    public async Task AddAuthorTest()
    {
        //Arrange
        Guid authorDtoId = Guid.NewGuid();
        string name = new Faker().Name.FullName();
        string username = new Faker().Internet.UserName(name);
        //Act
        await _mockChirpRepositories.AuthorRepository.AddAuthor(new AuthorDto
        {
            Id = authorDtoId,
            Name = name,
            Username = username,
            AvatarUrl = new Faker().Internet.Avatar()
        });
        //Assert
        _mockChirpRepositories.MockAuthorsDbSet.Verify(m => m.Add(It.IsAny<Author>()), Times.Once);
        _mockChirpRepositories.MockChirpDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        
    }

    [Fact] //Tests if the follow is added to an author
    public async Task AddFollowTest()
    {
        //Arrange
        Author authorToBeFollowed = _mockChirpRepositories.TestAuthors.Last();
        Author followingAuthor = _mockChirpRepositories.TestAuthors.First();
        //Act
        await _mockChirpRepositories.AuthorRepository.AddFollow(followingAuthor.AuthorId, authorToBeFollowed.AuthorId);
        //Assert
        Assert.Contains(authorToBeFollowed.FollowedBy,
            a => a.AuthorId.ToString().Equals(followingAuthor.AuthorId.ToString()));
    }

    [Fact]
    public async Task GetAuthorByEmailTest()
    {
        //Arrange
        Author author = _mockChirpRepositories.TestAuthors.First();
        //Act
        AuthorDto? resAuthor = await _mockChirpRepositories.AuthorRepository.GetAuthorFromUsername(author.Username);
        //Assert
        Assert.Equal(author.AuthorId, resAuthor?.Id);
    }

    [Fact]
    public async Task GetFollowsForAuthorTest()
    {
        //Arrange
        Author author = _mockChirpRepositories.TestAuthors.Last();
        //Act
        List<string> followerList = await _mockChirpRepositories.AuthorRepository.GetFollowsForAuthor(author.AuthorId);
        //Assert
        Assert.NotEmpty(followerList);
    }

    [Fact]
    public async Task RemoveFollowTest()
    {
        //Arrange
        Author author = _mockChirpRepositories.TestAuthors.First();
        Author unfollowAuthor = author.Follows.First();
        //Act
        await _mockChirpRepositories.AuthorRepository.RemoveFollow(author.AuthorId, unfollowAuthor.AuthorId);
        
        bool isFollowingRemoved = author.Follows.All(a => a.AuthorId != unfollowAuthor.AuthorId);
        bool isFollowedByRemoved = unfollowAuthor.FollowedBy.All(a => a.AuthorId != author.AuthorId);
        //Assert
        Assert.True(isFollowingRemoved);
        Assert.True(isFollowedByRemoved);
        _mockChirpRepositories.MockChirpDbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAuthorTest()
    {
        Author author = _mockChirpRepositories.TestAuthors.First();

        bool? isRemoved = await _mockChirpRepositories.AuthorRepository.DeleteAuthor(author.AuthorId);
        
        _mockChirpRepositories.MockAuthorsDbSet.Verify(m => m.Remove(It.IsAny<Author>()), Times.Once);
        _mockChirpRepositories.MockChirpDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        
        Assert.True(isRemoved);
    }
    
   
    
}

