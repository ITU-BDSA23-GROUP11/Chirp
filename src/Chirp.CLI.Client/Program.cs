namespace Chirp.CLI.Client;

using System;
using CSVDB;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

class Program
{
    static string filePath = @"chirp_db.csv";
    IDatabaseRepository db = new CsvDatabase(filePath);
    string userName = Environment.UserName;
    long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    int count = 0;
    
    public void Start(string[] args)
    {
        var rootCommand = new RootCommand("A simple command-line program");

        // defines the "read" command
        var readCommand = new Command("read", "Execute the read command");
        readCommand.Handler = CommandHandler.Create(ReadCommand);

        // defines the "cheep" command
        var cheepCommand = new Command("cheep", "Execute the cheep command");
        var messageOption = new Option<string>("--message", "The message to cheep");
        cheepCommand.AddOption(messageOption);
        cheepCommand.Handler = CommandHandler.Create(CheepCommand);
        
        // adds commands
        rootCommand.AddCommand(readCommand);
        rootCommand.AddCommand(cheepCommand);

        // parses the command line arguments and invokes the appropriate command
        rootCommand.Invoke(args);
    }

    void ReadCommand()
    {
        Console.WriteLine("Executing the 'read' command.");
        var cheeps = db.GetCheeps();
        foreach (var cheep in cheeps)
        {
            Console.WriteLine(cheep);
        }
    }

    void CheepCommand(string message)
    {
        Console.WriteLine(message);
        Console.WriteLine("Executing the 'cheep' command.");
            
        db.AddCheep(new Cheep(userName, message, timestamp));
    }
}

