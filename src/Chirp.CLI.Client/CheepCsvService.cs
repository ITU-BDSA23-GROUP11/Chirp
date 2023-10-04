using Chirp.CSVDB;
using Chirp.Utilities.Models;

namespace Chirp.CLI.Client;

public class CheepCsvService : ICheepService
{
    private static CheepCsvService? _instance;
    private readonly IDatabaseRepository _db = CsvDatabase.GetInstance();

    public static CheepCsvService GetInstance()
    {
        _instance ??= new CheepCsvService();
        return _instance;
    }

    private CheepCsvService()
    {
        
    }
    
    public async Task<Cheep[]> ReadCheeps()
    {
        return await Task.Run(() => _db.GetCheeps().ToArray());
    }

    public async Task WriteCheep(Cheep cheep)
    {
        await Task.Run(() => _db.AddCheep(cheep));
    }
}
