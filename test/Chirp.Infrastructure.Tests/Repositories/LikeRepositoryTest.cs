using Bogus;
using Chirp.Core.Dto;
using Chirp.Infrastructure.Models;
using Chirp.Tests.Core;
using Moq;
using NuGet.Frameworks;

namespace Chirp.Infrastructure.Tests.Repositories;

public class LikeRepositoryTest
{
    private readonly MockChirpRepositories _mockChirpRepositories = MockRepositoryFactory.GetMockCheepRepository();

    [Fact] //Tests if a like is added to the database
    public void LikeCheepTest() 
    {
        //Arrange
        Author author = _mockChirpRepositories.TestAuthors.First();
        Cheep cheep = _mockChirpRepositories.TestCheeps.First();
        
        //Act
        _mockChirpRepositories.LikeRepository.LikeCheep(author.AuthorId, cheep.CheepId);
        
        //Assert
        _mockChirpRepositories.MockLikesDbSet.Verify(m => m.Add(It.IsAny<Like>()), Times.Once);
        _mockChirpRepositories.MockChirpDbContext.Verify(m => m.SaveChanges(), Times.Once);
        List<LikeDto> likeListTestCheepId = _mockChirpRepositories.LikeRepository.GetLikesByCheepId(cheep.CheepId);
        List<LikeDto> likeListTestAuthorId = _mockChirpRepositories.LikeRepository.GetLikesByAuthorId(author.AuthorId);
        Assert.NotEmpty(likeListTestCheepId);
        Assert.NotEmpty(likeListTestAuthorId);
    }

    
    [Fact] //Tests if a like is removed from the database
    public void UnlikeCheepTest() 
    {
        //Arrange
        Author author = _mockChirpRepositories.TestAuthors.First();
        Cheep cheep = _mockChirpRepositories.TestCheeps.First();
        
        //Act
        _mockChirpRepositories.LikeRepository.LikeCheep(author.AuthorId, cheep.CheepId);
        if (_mockChirpRepositories.LikeRepository.IsLiked(author.AuthorId, cheep.CheepId))
        {
            _mockChirpRepositories.LikeRepository.UnlikeCheep(author.AuthorId, cheep.CheepId);
            
        }
        else
        {
            return;
        }
        _mockChirpRepositories.MockLikesDbSet.Verify(m => m.Remove(It.IsAny<Like>()), Times.Once());
        _mockChirpRepositories.MockChirpDbContext.Verify(m => m.SaveChanges(), Times.Once);
        
        //Assert
        bool liked = _mockChirpRepositories.LikeRepository.IsLiked(author.AuthorId, cheep.CheepId);
        Assert.False(liked);
    
    }
}