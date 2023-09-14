namespace Chirp.CLI.Client;


using System.Diagnostics;
using System.Text.RegularExpressions;

public class Program
{

    static string filePath = @"../../data/Chirp.CLI/chirp_cli_db.csv";
    static string userName = Environment.UserName;
    static long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    static int count = 0;
        
    static void Main(string[] args)
    {
        try {
            switch (args[0]) {
                case "read":
                    Read();
                    break;

                case "cheep":
                    Cheep(args.Skip(1).ToArray());
                    break;

                default:
                    Console.WriteLine("Error: Invalid command.");
                    break;
            }
        } catch (IndexOutOfRangeException e) {
            Console.WriteLine("Error: " + e.Message);
            Console.WriteLine("It appears that you did not specify a command.");
            Console.WriteLine("* Try: read or cheep");
        }
    }

    static void Read() {
        using (StreamReader sr = new StreamReader(filePath)) {
            sr.ReadLine();
            while (!sr.EndOfStream) {
                string line = sr.ReadLine();
                int firstComma = line.IndexOf(",");
                int lastComma = line.LastIndexOf(",");
                string readUser = line.Substring(0, firstComma);
                string readMessage = line.Substring(firstComma + 1, lastComma);
                long readTimestamp = long.Parse(line.Substring(lastComma + 1));
                Console.WriteLine(readUser + " @ " + timeStampConversion(readTimestamp) + ": " + readMessage);
                //string[] split = line.Split(",");
                //Console.WriteLine(split[2]);
                //Console.WriteLine(split[0] + " @ " + timeStampConversion(long.Parse(split[2])) + ": " + split[1]);
            }
        }
    }

    static void Cheep(string[] args) {
        if (args.Length == 0) {
            Console.WriteLine("Error: You did not apply content");
            return;
        }
        using (StreamWriter sw = new StreamWriter(filePath, true)) {
            string[] data = { userName, string.Join(" ", args), timestamp.ToString() };
            sw.WriteLine("\n" + string.Join(",", data));
        }
    }

    static string timeStampConversion(long unix) {
        DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(unix);
        
        string Date = dto.ToString("dd/MM/yyyy HH:mm:ss");
        return Date;
    }
    
}