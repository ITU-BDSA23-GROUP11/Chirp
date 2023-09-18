using CsvHelper;
using System.Globalization;
using System.Text.RegularExpressions;



namespace Chirp.CSVDB

{
    public record Cheep(string Author, string Message, long Timestamp);

    public class CsvDatabase : IDatabaseRepository
    {
        private string _filePath;

        public CsvDatabase(string filePath)
        {
            _filePath = filePath;
        }

        public void AddCheep(Cheep cheep)
        {
            using (var sw = new StreamWriter(_filePath, append: true))
            using (var csv = new CsvWriter(sw, CultureInfo.InvariantCulture))
            {
                csv.WriteRecord(cheep);
                sw.WriteLine();
            }
        }

        public List<String> GetCheeps()
        {
            List<string> cheepsList = new List<string>();

            using (StreamReader sr = new StreamReader(_filePath))
            using (var csv = new CsvReader(sr, CultureInfo.InvariantCulture))
            {
                while (csv.Read())
                {
                    var cheep = csv.GetRecord<Cheep>();
                    cheepsList.Add($"{cheep.Author} @ {TimeStampConversion(cheep.Timestamp)}: {cheep.Message}");
                }
            }

            return cheepsList;
        }
        public static string TimeStampConversion(long unix)
        {
            DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(unix).AddHours(2);//Account for time difference
            
            CultureInfo customCulture = new CultureInfo(CultureInfo.CurrentCulture.Name);
            customCulture.DateTimeFormat.DateSeparator = "/";
            customCulture.DateTimeFormat.TimeSeparator = ":";

            String date = dto.ToString("dd/MM/yy HH:mm:ss", customCulture);
            
            string regexPattern = @"^(0[1-9]|[1-2][0-9]|3[01])/(0[1-9]|1[0-2])/(\d{2}) (0[0-9]|1[0-9]|2[0-3]):([0-5][0-9]):([0-5][0-9])$";
            
            //Verify that the date is in the correct format
            bool matches = Regex.IsMatch(date, regexPattern);
            if (matches && dto.Year >= 1970) return date;
            else return "NaN";
        }
    }
}
