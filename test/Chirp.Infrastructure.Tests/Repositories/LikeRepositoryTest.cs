using Chirp.Core.Dto;
using Chirp.Infrastructure.Models;
using Chirp.Tests.Core;
using Moq;

namespace Chirp.Infrastructure.Tests.Repositories;

public class LikeRepositoryTest
{
    private readonly MockChirpRepositories _mockChirpRepositories = MockRepositoryFactory.GetMockCheepRepositories();

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
        
    }

    [Fact] //Tests if a cheep is already liked while trying to like it again. No duplicates should be made.
    public void AlreadyLikedCheepTest()
    {
        //Arrange
        Like like = _mockChirpRepositories.TestLikes.First();
        //Act
        _mockChirpRepositories.LikeRepository.LikeCheep(like.LikedByAuthor.AuthorId, like.Cheep.CheepId);
        //Assert
        _mockChirpRepositories.MockLikesDbSet.Verify(m => m.Add(It.IsAny<Like>()), Times.Never);
    }

    [Fact] //Tests the return of a list of likes based on a cheeps id. 
    public void GetLikesByCheepIdTest()
    {
        //Arrange
        Cheep cheep = _mockChirpRepositories.TestCheeps.First();
        //Act
        List<LikeDto> testList = _mockChirpRepositories.LikeRepository.GetLikesByCheepId(cheep.CheepId);
        //Assert
        Assert.NotEmpty(testList);
        
    }

    [Fact] //Tests the return of a list of likes based on an authors id
    public void GetLikesByAuthorId() 
    {
        //Arrange
        Author author = _mockChirpRepositories.TestAuthors.First();
        //Act
        List<LikeDto> testList = _mockChirpRepositories.LikeRepository.GetLikesByAuthorId(author.AuthorId);
        //Assert
        Assert.NotEmpty(testList);
        
    }

    [Fact] //Finds a like by authorId and cheepId
    public void GetLikeTest() 
    {
        //Arrange
        Like testLike = _mockChirpRepositories.TestLikes.First();
        //Act
        LikeDto testLikeDto = _mockChirpRepositories.LikeRepository.GetLike(testLike.LikedByAuthor.AuthorId, testLike.Cheep.CheepId);
        //Assert
        Assert.Equal(testLikeDto.LikedByAuthorId, testLike.LikedByAuthor.AuthorId);
        Assert.Equal(testLikeDto.CheepId, testLike.Cheep.CheepId);
    }

    [Fact] //Tests if a like exists with a boolean value
    public void IsLikedTest()
    {   
        //Arrange
        Like testLike = _mockChirpRepositories.TestLikes.First();
        //Act
        bool actualVal = _mockChirpRepositories.LikeRepository.IsLiked(testLike.LikedByAuthor.AuthorId, testLike.Cheep.CheepId);
        //Assert
        Assert.True(actualVal);
    }

    [Fact] //Tests of a like does not exist
    public void IsNotLikedTest()
    {
        Guid authorId = Guid.NewGuid();
        Guid cheepId = Guid.NewGuid();

        bool actualVal = _mockChirpRepositories.LikeRepository.IsLiked(authorId, cheepId);
        
        Assert.False(actualVal);
    }

    [Fact] //Tests if the method counts the amount of likes based on a cheep
    public void LikeCountTest()
    {
        bool isHigherThanZero = false;
        Like like = _mockChirpRepositories.TestLikes.First();

        int likeCount = _mockChirpRepositories.LikeRepository.LikeCount(like.Cheep.CheepId);
       
        if (likeCount > 0)
        {
            isHigherThanZero = true;
        }
        Assert.True(isHigherThanZero);
    }
    /*
    [Fact] //Tests if a like is removed from the database
    public void UnlikeCheepTest() 
    {
        //Arrange
        Like like = _mockChirpRepositories.TestLikes.First();
        //Act
        _mockChirpRepositories.LikeRepository.UnlikeCheep(like.LikedByAuthorId, like.CheepId);
        
        //Assert
        _mockChirpRepositories.MockLikesDbSet.Verify(m => m.Remove(like), Times.Once);
        _mockChirpRepositories.MockChirpDbContext.Verify(m => m.SaveChanges(), Times.Once);
        
        bool liked = _mockChirpRepositories.LikeRepository.IsLiked(like.LikedByAuthorId, like.CheepId);
        Assert.False(liked);
    
    }
    */
}