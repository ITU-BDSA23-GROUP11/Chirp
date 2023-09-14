using System.Text.RegularExpressions;
using System.Xml.XPath;
using Chirp.CLI.Client;
using Xunit;

namespace Chirp.CLI.Client.Tests;

public class Tests
{
    //UNITTEST -> Test that the UNIX time is converted to a string correctly
    [Test]
    public void TestUnixConversion()
    {
        //Arrange
        string regexPattern = @"^\d{2}/\d{2}/\d{2} \d{2}:\d{2}:\d{2}$";
        Program programClient = new Program();
        //Act
        string output = programClient.TimeStampConversion(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        bool regexMatch = Regex.IsMatch(output, regexPattern);
        //Assert
        Xunit.Assert.True(regexMatch);
    }
}