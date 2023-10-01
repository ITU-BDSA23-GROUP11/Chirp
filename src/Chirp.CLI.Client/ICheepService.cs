namespace Chirp.CLI.Client;

public record Cheep(string Author, string Message, long Timestamp);

public interface ICheepService
{
    public Task<Cheep[]> ReadCheeps();

    public Task WriteCheep(Cheep cheep);
}