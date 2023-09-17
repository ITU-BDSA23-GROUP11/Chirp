﻿namespace Chirp.CLI.Client;

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
    public int start(string[] args)
    {
        argsToBeUsed = args;
        var rootCommand = new RootCommand("A simple command-line program");

        // Define the "read" command
        var readCommand = new Command("read", "Execute the read command");
        readCommand.Handler = CommandHandler.Create(ReadCommand);

        // Define the "cheep" command
        var cheepCommand = new Command("cheep", "Execute the cheep command");
        var messageOption = new Option<string>("--message", "The message to cheep");
        cheepCommand.AddOption(messageOption);
        cheepCommand.Handler = CommandHandler.Create(CheepCommand);

        // Add the commands to the root command
        rootCommand.AddCommand(readCommand);
        rootCommand.AddCommand(cheepCommand);

        // Parse the command-line arguments and invoke the appropriate command
        return rootCommand.Invoke(args);
    }

    void ReadCommand()
    {
        Console.WriteLine("Executing the 'read' command.");
        UserInterface.Read(filePath);
    }

    void CheepCommand(string message)
    {
        Console.WriteLine("Executing the 'cheep' command.");
        CheepWrite(argsToBeUsed.Skip(1).ToArray());
        // Add your 'cheep' command logic here
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




/*
try {
    switch (args[0]) {
        case "read":
            Read();
            break;

        case "cheep":
            
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
*/


