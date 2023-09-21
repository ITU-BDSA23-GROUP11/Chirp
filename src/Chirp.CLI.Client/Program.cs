namespace Chirp.CLI.Client;

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Chirp.CSVDB;
using System.Globalization;
using System.CommandLine;
using System;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

class Program
{
    string filePath = @"C:\Users\laust\RiderProjects\ProjectFolder\Chirp\src\Chirp.CLI.Client\chirp_cli_db.csv";
    string userName = Environment.UserName;
    long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    int count = 0;
    string[] argsToBeUsed = null;
    public int Start(string[] args)
    {
        argsToBeUsed = args;
        var rootCommand = new RootCommand("A simple command-line program");

        // defines the "read" command
        var readCommand = new Command("read", "Execute the read command");
        readCommand.Handler = CommandHandler.Create(ReadCommand);

        // defines the "cheep" command
        var cheepCommand = new Command("cheep", "Execute the cheep command");
        var messageOption = new Option<string>("--message", "The message to cheep");
        cheepCommand.AddOption(messageOption);
        cheepCommand.Handler = CommandHandler.Create(CheepCommand);
        
        rootCommand.AddCommand(readCommand);
        rootCommand.AddCommand(cheepCommand);

        // parses the command line arguments and invokes the appropriate command
        return rootCommand.Invoke(args);
    }

    void ReadCommand()
    {
        Console.WriteLine("Executing the 'read' command.");
        UserInterface.Read(filePath);
    }

    void CheepCommand(string message)
    {
        Console.WriteLine(message);
        Console.WriteLine("Executing the 'cheep' command.");
        CheepWrite(message.Split());
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
    
    void Cheep(string[] args)
    {
        if (args.Length == 0) {
            Console.WriteLine("Error: You did not apply content");
            return;
        }
        using (StreamWriter sw = new StreamWriter(filePath, true)) {
            string[] data = { userName, string.Join(" ", args), timestamp.ToString() };
            sw.WriteLine(string.Join(",", data));
        }
    }
}

