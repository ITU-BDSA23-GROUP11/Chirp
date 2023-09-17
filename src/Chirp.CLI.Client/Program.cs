namespace Chirp.CLI.Client;

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Chirp.CSVDB;
using System.Globalization;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

public class Program
{

    static string filePath = @"../../data/Chirp.CLI/chirp_cli_db.csv";
    IDatabaseRepository db = new CsvDatabase(filePath);
    static string userName = Environment.UserName;
    static long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    static int count = 0;
    
    public record Cheep(string Author, string Message, long Timestamp);
        
    static void Main(string[] args)
    {
        try
        {
            switch (args[0])
            {
                case "read":
                    UserInterface.Read(filePath);
                    break;

                case "cheep":
                    CheepWrite(args.Skip(1).ToArray());
                    break;

                default:
                    Console.WriteLine("Error: Invalid command.");
                    break;
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