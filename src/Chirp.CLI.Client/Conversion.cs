using System.Globalization;

namespace Chirp.CLI.Client;

public class Conversion
{
    public static string TimeStampConversion(long unix) {
        DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(unix);
        
        //Set up a custom culture to ensure that formatting is in accordance with requested
        CultureInfo customCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        customCulture.DateTimeFormat.TimeSeparator = ":";
        
        String date = dto.ToString("dd/MM/yy HH:mm:ss", customCulture);
        return date;
    }
}