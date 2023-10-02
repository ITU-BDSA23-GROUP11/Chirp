using CsvHelper;
using System.Globalization;
using Chirp.Utilities.Models;

namespace Chirp.CSVDB
{
    public class CsvDatabase : IDatabaseRepository
    {
        private static CsvDatabase? _instance;
        private readonly string _filePath;

        private CsvDatabase(string filePath)
        {
            _filePath = filePath;
            if (!File.Exists(filePath))
            {
                _filePath = "./chirp_db.csv";
                if (!File.Exists(_filePath))
                {
                    File.Create(_filePath).Close();
                    File.WriteAllText(_filePath, "Author,Message,Timestamp");
                }
            }
            else
            {
                Console.WriteLine("");
            }
        }

        public static CsvDatabase GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CsvDatabase("../../dat/chirp_db.csv");
            }

            return _instance;
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

        public List<Cheep> GetCheeps()
        {
            List<Cheep> cheepsList = new List<Cheep>();

            using (StreamReader sr = new StreamReader(_filePath))
            using (var csv = new CsvReader(sr, CultureInfo.InvariantCulture))
            {
                while (csv.Read())
                {
                    var cheep = csv.GetRecord<Cheep>();
                    
                    if (cheep != null) cheepsList.Add(cheep);
                }
            }

            return cheepsList;
        }
    }
}
