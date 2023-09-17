namespace Chirp.CLI.Client;

public class App
{
    static void Main(string[] args)
    {
        //Initialize client and run program with argument
        Program client = new Program();
        client.start(args);
        
    }
}