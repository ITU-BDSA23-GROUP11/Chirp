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