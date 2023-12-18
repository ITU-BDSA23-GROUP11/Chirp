using System.ComponentModel.DataAnnotations;
using Bogus;
using Chirp.Infrastructure.Models;
using Chirp.Tests.Core;

namespace Chirp.Infrastructure.Tests.Models;

public class ModelsUnitTests
{
    [Fact]
    public void TestAuthorFields()
    {
        string name = new Faker().Name.FullName();
        string username = new Faker().Internet.UserName(name);
        string avatarUrl = new Faker().Internet.Avatar();

        Author author = new Author
        {
            Name = name,
            Username = username,
            AvatarUrl = avatarUrl
        };

        Assert.Equal(name, author.Name);
        Assert.Equal(username, author.Username);
        Assert.Equal(avatarUrl, author.AvatarUrl);
        Assert.Equal(Guid.Empty, author.AuthorId);
        Assert.Empty(author.Cheeps);
    }

    [Fact]
    public void TestLikeFields()
    {
        var author = DataGenerator.GenerateAuthorFaker().Generate();
        var cheepId = Guid.NewGuid();
        var cheep = new Cheep
        {
            CheepId = cheepId,
            Author = author,
            Text = new Faker().Random.Words()
        };
        Like like = new Like
        {
            LikedByAuthor = author,
            Cheep = cheep
        };
        
        Assert.Equal(author.AuthorId.ToString(), like.LikedByAuthor.AuthorId.ToString());
        Assert.Equal(cheepId.ToString(), like.Cheep.CheepId.ToString());
    }

    [Fact]
    public void TestCheepFields()
    {
        Author author = DataGenerator.GenerateAuthorFaker().Generate();

        string text = new Faker().Random.Words();

        Cheep cheep = new Cheep
        {
            Author = author,
            Text = text
        };

        Assert.Equal(text, cheep.Text);
        Assert.Equal(author.AuthorId, cheep.Author.AuthorId);
    }

    [Fact]
    public void TestCheepModelCorrectTimestamp()
    {
        var author = DataGenerator.GenerateAuthorFaker().Generate();
        var cheep = new Cheep
        {
            Author = author,
            Text = new Faker().Random.Words()
        };
        var timeNowMinusOneSecond = DateTime.UtcNow.Add(TimeSpan.FromSeconds(-1)).ToFileTimeUtc();
        var cheepTime = cheep.Timestamp.ToFileTimeUtc();

        Assert.True(cheepTime > timeNowMinusOneSecond);
    }

    [Fact]
    public void ExceptionTestNameLengthMin()
    {
        var author = new Author
        {
            Name = "1234",
            Username = new Faker().Internet.UserName(),
            AvatarUrl = new Faker().Internet.Avatar()
        };

        var exception = Assert.Throws<System.ComponentModel.DataAnnotations.ValidationException>
            (() => Validator.ValidateObject(author, new ValidationContext(author), true));
        Assert.Contains("Username must contain more than 5 characters", exception.Message);
    }

    [Fact]
    public void ExceptionTestNameLengthMax()
    {
        var author = new Author
        {
            Name = new string('a', 51),
            Username = new Faker().Internet.UserName(),
            AvatarUrl = new Faker().Internet.Avatar()

        };

        var exception = Assert.Throws<System.ComponentModel.DataAnnotations.ValidationException>
            (() => Validator.ValidateObject(author, new ValidationContext(author), true));
        Assert.Contains("Username must contain less than 50 characters", exception.Message);
    }

    [Fact]
    public void ExceptionTestCheepLengthMax()
    {
        var author = DataGenerator.GenerateAuthorFaker().Generate();
        Cheep cheep = new Cheep
        {
            Author = author,
            Text = new string('a', 161)
        };

        var exception = Assert.Throws<System.ComponentModel.DataAnnotations.ValidationException>
            (() => Validator.ValidateObject(cheep, new ValidationContext(cheep), true));
        Assert.Contains("Cheeps must contain less than 160 characters", exception.Message);
    }

    [Fact]
    public void ExceptionTestCheepLengthMin()
    {
        var author = DataGenerator.GenerateAuthorFaker().Generate();
        Cheep cheep = new Cheep
        {
            Author = author,
            Text = ""
        };

        var exception = Assert.Throws<System.ComponentModel.DataAnnotations.ValidationException>
            (() => Validator.ValidateObject(cheep, new ValidationContext(cheep), true));
        Assert.Contains("The Text field is required", exception.Message);
    }

}