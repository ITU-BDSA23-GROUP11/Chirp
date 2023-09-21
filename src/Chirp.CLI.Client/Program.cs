using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Chirp.CSVDB;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Chirp.CLI
{
    class Program
    {
        static string filePath = @"chirp_db.csv";
        static IDatabaseRepository db = new CsvDatabase(filePath); //redundant?
        public record Cheep(string Author, string Message, long Timestamp);

        public static async Task Main(string[] args)
        {
            try
            {
                switch (args[0])
            {
                case "read":
                    await Read();
                    break;

                    case "cheep":
                    await CheepWrite(args.Skip(1).ToArray());
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

        static async Task Read()
    {
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri("http://localhost:5012");
        var cheeps = await client.GetFromJsonAsync<string[]>("/cheeps");
        foreach (var cheep in cheeps)
        {
            Console.WriteLine(cheep);
        }
    }

    static async Task CheepWrite(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Error: You did not supply any content");
            return;
        }

        string userName = Environment.UserName;
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        Cheep newCheep = new Cheep(userName, string.Join(" ", args), timestamp);

        using HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("http://localhost:5012");  
        var response = await client.PostAsJsonAsync("/cheep", newCheep);
    
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Successfully sent the cheep!");
        }
        else
        {
            Console.WriteLine($"Failed to send cheep. Status code: {response.StatusCode}");
        }
    }

    }
}
