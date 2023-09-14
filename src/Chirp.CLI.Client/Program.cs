namespace Chirp.CLI.Client;

using Chirp.CSVDB;
using System.Globalization;

public class Program
{

    string filePath = @"../../data/Chirp.CLI/chirp_cli_db.csv";
    string userName = Environment.UserName;
    long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    int count = 0;
    
    public void Main(string[] args)
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

    void Read() {
        using (StreamReader sr = new StreamReader(filePath)) {
            sr.ReadLine();
            while (!sr.EndOfStream) {
                string line = sr.ReadLine();
                int firstComma = line.IndexOf(",");
                int lastComma = line.LastIndexOf(",");
                string readUser = line.Substring(0, firstComma);
                string readMessage = line.Substring(firstComma + 1, lastComma);
                long readTimestamp = long.Parse(line.Substring(lastComma + 1));
                Console.WriteLine(readUser + " @ " + TimeStampConversion(readTimestamp) + ": " + readMessage);
                //string[] split = line.Split(",");
                //Console.WriteLine(split[2]);
                //Console.WriteLine(split[0] + " @ " + timeStampConversion(long.Parse(split[2])) + ": " + split[1]);
            }
        }
    }

    void Cheep(string[] args) {
        if (args.Length == 0) {
            Console.WriteLine("Error: You did not apply content");
            return;
        }
        using (StreamWriter sw = new StreamWriter(filePath, true)) {
            string[] data = { userName, string.Join(" ", args), timestamp.ToString() };
            sw.WriteLine("\n" + string.Join(",", data));
        }
    }

    public string TimeStampConversion(long unix) {
        DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(unix);
        
        //Set up a custom culture to ensure that formatting is in accordance with requested
        CultureInfo customCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        customCulture.DateTimeFormat.TimeSeparator = ":";
        
        String date = dto.ToString("dd/MM/yy HH:mm:ss", customCulture);
        return date;
    }
    
}