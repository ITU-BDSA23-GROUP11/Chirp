using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Chirp.CSVDB
{
    public record Cheep(string Author, string Message, long Timestamp);

    public class CsvDatabase : IDatabaseRepository
    {
        private static CsvDatabase _instance;
        private static readonly object _lock = new object();

        private string _filePath;

        private CsvDatabase(string filePath)
        {
            _filePath = filePath;
        }

        public static CsvDatabase Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CsvDatabase("chirp_db.csv");
                        }
                    }
                }
                return _instance;
            }
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

        public List<string> GetCheeps()
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