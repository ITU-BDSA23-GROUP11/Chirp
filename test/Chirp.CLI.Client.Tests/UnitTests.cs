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
        String val = "14/09/2023 13:06:50";
        Program programClient = new Program();
        //Act
        String output = programClient.TimeStampConversion(1694696810);
        //Assert
        Xunit.Assert.Equal(val, output);
    }
}