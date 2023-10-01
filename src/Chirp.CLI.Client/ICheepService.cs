using Chirp.Utilities.Models;

namespace Chirp.CLI.Client;

public interface ICheepService
{
    public Task<Cheep[]> ReadCheeps();

    public Task WriteCheep(Cheep cheep);
}