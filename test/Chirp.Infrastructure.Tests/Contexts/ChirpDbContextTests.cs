using Bogus;
using Chirp.Infrastructure.Contexts;
using Chirp.Infrastructure.Models;
using Chirp.Tests.Core;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Chirp.Infrastructure.Tests.Contexts;

public class ChirpDbContextTests : IClassFixture<ChirpDbContextFixture>
{
    private readonly ChirpDbContextFixture _fixture;

    public ChirpDbContextTests(ChirpDbContextFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public void TestModelsIdGenerationUnique()
    {
        ChirpDbContext context = _fixture.GetContext();

        List<Author> authors = DataGenerator.GenerateAuthorFaker(false).GenerateBetween(1, 10);
        List<Cheep> cheeps = DataGenerator.GenerateCheepFaker(authors, false).GenerateBetween(50, 100);
        
        // Guids should not have been set yet
        Guid emptyGuid = Guid.Empty;
        foreach (Author author in authors)
        {
            Assert.Equal(emptyGuid, author.AuthorId);
        }
        foreach (Cheep cheep in cheeps)
        {
            Assert.Equal(emptyGuid, cheep.CheepId);
        }
        
        context.Authors.AddRange(authors);
        context.Cheeps.AddRange(cheeps);
        context.SaveChanges();
        
        // Guids should now be set and unique
        List<Guid> authorIds = new List<Guid>();
        foreach (Author author in authors)
        {
            Assert.NotEqual(emptyGuid, author.AuthorId);
            Assert.True(!authorIds.Contains(author.AuthorId));
            authorIds.Add(author.AuthorId);
        }
        
        List<Guid> cheepIds = new List<Guid>();
        foreach (Cheep cheep in cheeps)
        {
            Assert.NotEqual(emptyGuid, cheep.CheepId);
            Assert.True(!cheepIds.Contains(cheep.CheepId));
            cheepIds.Add(cheep.CheepId);
        }
        
        context.Cheeps.RemoveRange(cheeps);
        context.Authors.RemoveRange(authors);
        context.SaveChanges();
    }

    [Fact]
    public void TestGetCheeps()
    {
        ChirpDbContext context = _fixture.GetContext();

        List<Cheep> cheeps = context.Cheeps.ToList();
        foreach (var cheep in cheeps)
        {
            Assert.NotNull(cheep);
            Assert.IsType<Cheep>(cheep);
        }
        
        Assert.Equal(_fixture.Data.Cheeps.Count, cheeps.Count);
    }
    
    [Fact]
    public void TestGetAuthors()
    {
        ChirpDbContext context = _fixture.GetContext();

        List<Author> authors = context.Authors.ToList();
        foreach (var author in authors)
        {
            Assert.NotNull(author);
            Assert.IsType<Author>(author);
        }
        Assert.Equal(_fixture.Data.Authors.Count, authors.Count);
    }
    
    [Fact]
    public void TestAddCheepAuthorAndLike()
    {
        ChirpDbContext context = _fixture.GetContext();
        
        Author author = DataGenerator.GenerateAuthorFaker(false).Generate();
        
        Cheep cheep = new Cheep
        {
            Author = author,
            Text = new Faker().Random.Words(),
            Timestamp = new Faker().Date.Past()
        };

        Like like = new Like
        {
            LikedByAuthor = author,
            Cheep = cheep
        };

        Comment comment = new Comment
        {
            CommentAuthor = author,
            Cheep = cheep,
            Text = new Faker().Random.Words()
        };

        EntityEntry<Author> addedAuthor = context.Authors.Add(author);
        EntityEntry<Cheep> addedCheep = context.Cheeps.Add(cheep);
        EntityEntry<Like> addedLike = context.Likes.Add(like);
        EntityEntry<Comment> addedComment = context.Comments.Add(comment);
        
        Assert.Equal(author.Name, addedAuthor.Entity.Name);
        Assert.Equal(author.Name, addedCheep.Entity.Author.Name);
        Assert.Equal(cheep.Author, author);
        Assert.Equal(cheep.Text, addedCheep.Entity.Text);
        Assert.Equal(like.LikedByAuthor.AuthorId, addedLike.Entity.LikedByAuthor.AuthorId);
        Assert.Equal(comment.CommentAuthor.AuthorId, addedComment.Entity.CommentAuthor.AuthorId);
        Assert.Equal(comment.Cheep.CheepId, addedComment.Entity.Cheep.CheepId);
    }
}