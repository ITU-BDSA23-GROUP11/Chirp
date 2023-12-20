using Bogus;
using Chirp.Core.Dto;
using Chirp.Infrastructure.Models;
using Chirp.Tests.Core;
using Moq;

namespace Chirp.Infrastructure.Tests.Repositories;

public class LikeRepositoryTest
{
    private readonly MockChirpRepositories _mockChirpRepositories = MockRepositoryFactory.GetMockCheepRepositories();

    [Fact] //Tests if a like is added to the database
    public async Task LikeCheepTest() 
    {
        //Arrange
        Cheep cheep = _mockChirpRepositories.TestCheeps.First();
        cheep.Likes.Clear();

        Author author = _mockChirpRepositories.TestAuthors.First();
        author.Likes.Clear();
        
        //Act
        await _mockChirpRepositories.LikeRepository.LikeCheep(author.AuthorId, cheep.CheepId);
        
        //Assert
        _mockChirpRepositories.MockLikesDbSet.Verify(m => m.Add(It.IsAny<Like>()), Times.Once);
        _mockChirpRepositories.MockChirpDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        
    }

    [Fact] //Tests if a cheep is already liked while trying to like it again. No duplicates should be made.
    public async Task AlreadyLikedCheepTest()
    {
        //Arrange
        Like like = _mockChirpRepositories.TestLikes.First();
        //Act
        await _mockChirpRepositories.LikeRepository.LikeCheep(like.LikedByAuthor.AuthorId, like.Cheep.CheepId);
        //Assert
        _mockChirpRepositories.MockLikesDbSet.Verify(m => m.Add(It.IsAny<Like>()), Times.Never);
    }

    [Fact] //Tests the return of a list of likes based on a cheeps id. 
    public async Task GetLikesByCheepIdTest()
    {
        //Arrange
        Cheep cheep = _mockChirpRepositories.TestLikes.First().Cheep;
        //Act
        List<LikeDto> testList = await _mockChirpRepositories.LikeRepository.GetLikesByCheepId(cheep.CheepId);
        //Assert
        Assert.NotEmpty(testList);
        
    }

    [Fact] //Tests the return of a list of likes based on an authors id
    public async Task GetLikesByAuthorId() 
    {
        //Arrange
        Author author = _mockChirpRepositories.TestLikes.First().LikedByAuthor;
        //Act
        List<LikeDto> testList = await _mockChirpRepositories.LikeRepository.GetLikesByAuthorId(author.AuthorId);
        //Assert
        Assert.NotEmpty(testList);
        
    }

    [Fact] //Finds a like by authorId and cheepId
    public async Task GetLikeTest() 
    {
        //Arrange
        Like testLike = _mockChirpRepositories.TestLikes.First();
        //Act
        LikeDto testLikeDto = await _mockChirpRepositories.LikeRepository.GetLike(testLike.LikedByAuthor.AuthorId, testLike.Cheep.CheepId);
        //Assert
        Assert.Equal(testLikeDto.LikedByAuthorId, testLike.LikedByAuthor.AuthorId);
        Assert.Equal(testLikeDto.CheepId, testLike.Cheep.CheepId);
    }

    [Fact] //Tests if a like exists with a boolean value
    public async Task IsLikedTest()
    {   
        //Arrange
        Like testLike = _mockChirpRepositories.TestLikes.First();
        //Act
        bool actualVal = await _mockChirpRepositories.LikeRepository.IsLiked(testLike.LikedByAuthor.AuthorId, testLike.Cheep.CheepId);
        //Assert
        Assert.True(actualVal);
    }

    [Fact] //Tests of a like does not exist
    public async Task IsNotLikedTest()
    {
        Guid authorId = Guid.NewGuid();
        Guid cheepId = Guid.NewGuid();

        bool actualVal = await _mockChirpRepositories.LikeRepository.IsLiked(authorId, cheepId);
        
        Assert.False(actualVal);
    }

    [Fact] //Tests if the method counts the amount of likes based on a cheep
    public async Task LikeCountTest()
    {
        bool isHigherThanZero = false;
        Like like = _mockChirpRepositories.TestLikes.First();

        int likeCount = await _mockChirpRepositories.LikeRepository.LikeCount(like.Cheep.CheepId);
       
        if (likeCount > 0)
        {
            isHigherThanZero = true;
        }
        Assert.True(isHigherThanZero);
    }

    [Fact] //Tests if a like is removed from the database
    public async Task UnlikeCheepTest() 
    {
        //Arrange
        Like like = _mockChirpRepositories.TestLikes.First();
        Guid cheepId = like.Cheep.CheepId;
        Guid authorId = like.LikedByAuthor.AuthorId;
        //Act
        await _mockChirpRepositories.LikeRepository.UnlikeCheep(authorId, cheepId);
        
        //Assert
        _mockChirpRepositories.MockLikesDbSet.Verify(m => m.Remove(It.IsAny<Like>()), Times.Once);
        _mockChirpRepositories.MockChirpDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        
    }
    
}