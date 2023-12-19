using Bogus;
using Chirp.Core.Dto;
using Chirp.Infrastructure.Models;
using Chirp.Tests.Core;
using Moq;

namespace Chirp.Infrastructure.Tests.Repositories;

public class CommentRepositoryTest
{
    private readonly MockChirpRepositories _mockChirpRepositories = MockRepositoryFactory.GetMockCheepRepositories();

    [Fact]
    public void TestAddComment()
    {
        Author author = _mockChirpRepositories.TestAuthors.First();
        
        Cheep cheep = _mockChirpRepositories.TestCheeps.First();

        AddCommentDto comment = new AddCommentDto
        {
            AuthorId = author.AuthorId,
            CheepId = cheep.CheepId,
            Text = new Faker().Random.Words()
        };

        bool? isAdded = _mockChirpRepositories.CommentRepository.AddComment(comment);
        
        _mockChirpRepositories.MockCommentsDbSet.Verify(m => m.Add(It.IsAny<Comment>()), Times.Once);
        _mockChirpRepositories.MockChirpDbContext.Verify(m => m.SaveChanges(), Times.Once);
        
        Assert.True(isAdded);
        Assert.Equal(author.AuthorId, comment.AuthorId);
        Assert.Equal(cheep.CheepId, comment.CheepId);
        
    }

    [Fact]
    public void TestDeleteComment()
    {
        Comment comment = _mockChirpRepositories.TestComment.First();

        Guid commentIdToDelete = comment.CommentId;
        
        bool? isDeleted = _mockChirpRepositories.CommentRepository.DeleteComment(commentIdToDelete);
        
        _mockChirpRepositories.MockChirpDbContext.Verify(m => m.Remove(It.IsAny<Comment>()), Times.Once);
        _mockChirpRepositories.MockChirpDbContext.Verify(m => m.SaveChanges());
        
        Assert.True(isDeleted);
    }

   
    
}