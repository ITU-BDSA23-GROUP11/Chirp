using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Chirp.CSVDB;

namespace Chirp.CLI
{
    public class Program
    {
        static string filePath = @"chirp_db.csv";
        IDatabaseRepository db = new CsvDatabase(filePath);
        string userName = Environment.UserName;
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        public record Cheep(string Author, string Message, long Timestamp);

        public void Main(string[] args)
        {
            try
            {
                switch (args[0])
                {
                    case "read":
                        Read();
                        break;

                    case "cheep":
                        CheepWrite(args.Skip(1).ToArray());
                        break;

                    default:
                        Console.WriteLine("Error: Invalid command.");
                        break;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine("Error: " + e.Message);
                Console.WriteLine("It appears that you did not specify a command.");
                Console.WriteLine("* Try: read or cheep");
            }
        }

        void Read()
        {
            var cheeps = db.GetCheeps();
            foreach (var cheep in cheeps)
            {
                Console.WriteLine(cheep);
            }
        }

        void CheepWrite(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Error: You did not apply content");
                return;
            }
            
            db.AddCheep(new Chirp.CSVDB.Cheep(userName, string.Join(" ", args), timestamp));

        }
    }
}