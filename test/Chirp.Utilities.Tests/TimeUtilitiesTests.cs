namespace Chirp.Utilities.Tests;

public class TimeUtilitiesTests
{
    [Theory]
    [InlineData("18/09/23 14:38:59", 1695040739, true)]
    [InlineData("NaN", -100000000, true)]
    [InlineData("18/09/23 14:38:59", 1692440719, false)]
    [InlineData("NaN", 1, false)]
    public void TextUnixConversionEqual(String timestamp, long unix, bool expectedToMatch)
    {
        String converted = TimeUtilities.TimeStampConversion(unix);
        bool match = timestamp.Equals(converted);
        
        Assert.Equal(match, expectedToMatch);
    }
}