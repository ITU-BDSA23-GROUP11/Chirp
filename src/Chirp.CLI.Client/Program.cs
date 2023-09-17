namespace Chirp.CLI.Client;

using Chirp.CSVDB;
using System.Globalization;

public class Program
{
    string filePath = @"C:\Users\laust\RiderProjects\ProjectFolder\Chirp\src\Chirp.CLI.Client\chirp_cli_db.csv";
    string userName = Environment.UserName;
    long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    int count = 0;
    
    public void Main(string[] args)
    {
        try {
            switch (args[0]) {
                case "read":
                    UserInterface.Read(filePath);
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
    
}