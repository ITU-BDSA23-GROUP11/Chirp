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
            using (var sw = new StreamWriter(@_filePath, append: true))
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
                    cheepsList.Add($"{cheep.Author} @ {Conversion.TimeStampConversion(cheep.Timestamp)}: {cheep.Message}");
                }
            }

            return cheepsList;
        }
    }
}
