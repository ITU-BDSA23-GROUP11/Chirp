using System.Globalization;
using System.Text.RegularExpressions;

namespace Chirp.Utilities;

public class TimeUtilities
{
    public static string TimeStampConversion(long unix) {
        DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(unix).AddHours(2);//Account for time difference
            
        CultureInfo customCulture = new CultureInfo(CultureInfo.CurrentCulture.Name);
        customCulture.DateTimeFormat.DateSeparator = "/";
        customCulture.DateTimeFormat.TimeSeparator = ":";

        String date = dto.ToString("dd/MM/yy HH:mm:ss", customCulture);
            
        string regexPattern = @"^(0[1-9]|[1-2][0-9]|3[01])/(0[1-9]|1[0-2])/(\d{2}) (0[0-9]|1[0-9]|2[0-3]):([0-5][0-9]):([0-5][0-9])$";
            
        //Verify that the date is in the correct format
        bool matches = Regex.IsMatch(date, regexPattern);

        return (matches && dto.Year >= 1970) ? date : "NaN";
    }
}