using Chirp.DBService.Contexts;
using Chirp.DBService.Models;
using Chirp.DBService.Tests.Fixtures;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Chirp.DBService.Tests.Contexts;

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
        
        var author1 = new Author
        {
            Name = "Kim",
            Email = "kim@itu.dk"
        };
        var author2 = new Author
        {
            Name = "Kim",
            Email = "kim@itu.dk"
        };
        var cheep1 = new Cheep
        {
            Author = author1,
            Text = "Hello Message"
        };
        
        var cheep2 = new Cheep
        {
            Author = author2,
            Text = "Hello Message"
        };

        Guid defaultGuid = cheep1.CheepId;

        // Shouldn't be able to overwrite id, as it can only be set when inserted in the DB
        cheep1.CheepId = new Guid();
        cheep2.CheepId = new Guid();
        author1.AuthorId = new Guid();
        author2.AuthorId = new Guid();
        
        // Expected as the models haven't been inserted yet
        Assert.Equal(cheep1.CheepId, cheep2.CheepId);
        
        context.Authors.AddRange(author1, author2);
        context.Cheeps.AddRange(cheep1, cheep2);
        context.SaveChanges();
        
        Assert.NotEqual(defaultGuid.ToString(), cheep1.CheepId.ToString());
        Assert.NotEqual(defaultGuid.ToString(), cheep2.CheepId.ToString());
        Assert.NotEqual(cheep1.CheepId.ToString(), cheep2.CheepId.ToString());
        Assert.NotEqual(defaultGuid.ToString(), author1.AuthorId.ToString());
        Assert.NotEqual(author1.AuthorId.ToString(), author2.AuthorId.ToString());
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
        Assert.Equal(12, cheeps.Count);
    }
    
    [Fact]
    public void TestAddCheepAndAuthor()
    {
        ChirpDbContext context = _fixture.GetContext();
        
        Author author = new Author
        {
            Name = "Kim",
            Email = "kim@itu.dk"
        };
        Cheep cheep = new Cheep
        {
            Author = author
        };

        EntityEntry<Author> addedAuthor = context.Authors.Add(author);
        EntityEntry<Cheep> addedCheep = context.Cheeps.Add(cheep);
        
        Assert.Equal(author.Name, addedAuthor.Entity.Name);
        Assert.Equal(author.Name, addedCheep.Entity.Author.Name);
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
        Assert.Equal(2, authors.Count);
    }
}