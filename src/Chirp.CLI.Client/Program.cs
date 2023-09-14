using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Chirp.CSVDB;

namespace Chirp.CLI
{
    class Program
    {
        static string filePath = @"./data/chirp_db.csv";
        static IDatabaseRepository db = new CsvDatabase(filePath);
        static string userName = Environment.UserName;
        static long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        public record Cheep(string Author, string Message, long Timestamp);

        public static void Main(string[] args)
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

        static void Read()
        {
            var cheeps = db.GetCheeps();
            foreach(var cheep in cheeps)
            {
                Console.WriteLine(cheep);
            }
        }

        static void CheepWrite(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Error: You did not apply content");
                return;
            }
            //string[] data = { userName, "\"" + string.Join(" ", args) + "\"", timestamp };
            db.AddCheep(new Chirp.CSVDB.Cheep(userName, string.Join(" ", args), timestamp));
            
        }

    }
}
