using System.ComponentModel.DataAnnotations;
using Bogus;
using Chirp.Infrastructure.Models;
using Chirp.Infrastructure.Tests.Utilities;
using ValidationResult = Bogus.ValidationResult;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Chirp.Infrastructure.Tests.Models;

public class ModelsUnitTests
{
    [Fact]
    public void TestAuthorFields()
    {
        string name = new Faker().Name.FullName();
        string email = new Faker().Internet.Email(name);

        Author author = new Author
        {
            Name = name,
            Email = email
        };

        Assert.Equal(name, author.Name);
        Assert.Equal(email, author.Email);
        Assert.Equal(Guid.Empty, author.AuthorId);
        Assert.Empty(author.Cheeps);
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
        Assert.Equal(author.Name, cheep.Author.Name);
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
            Email = "test@email.com"
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
            Email = "test@email.com"

        };

        var exception = Assert.Throws<System.ComponentModel.DataAnnotations.ValidationException>
            (() => Validator.ValidateObject(author, new ValidationContext(author), true));
        Assert.Contains("Username must contain less than 50 characters", exception.Message);
    }

    [Fact]
    public void ExceptionTestEmailFormat()
    {
        var author = new Author
        {
            Name = "testingEmail",
            Email = "FailMail"
        };
        var exception = Assert.Throws<System.ComponentModel.DataAnnotations.ValidationException>
            (() => Validator.ValidateObject(author, new ValidationContext(author), true));
        Assert.Contains("Invalid email format.", exception.Message);
    }

}