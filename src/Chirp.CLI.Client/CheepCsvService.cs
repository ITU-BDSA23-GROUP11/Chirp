using Chirp.CSVDB;
using Chirp.Utilities.Models;

namespace Chirp.CLI.Client;

public class CheepCsvService : ICheepService
{
    private static CheepCsvService? _instance;
    private readonly IDatabaseRepository db = CsvDatabase.Instance;

    public static CheepCsvService GetInstance()
    {
        if (_instance == null)
        {
            _instance = new CheepCsvService();
        }

        return _instance;
    }

    private CheepCsvService()
    {
        
    }
    
    public async Task<Cheep[]> ReadCheeps()
    {
        return db.GetCheeps().ToArray();
    }

    public async Task WriteCheep(Cheep cheep)
    {
        db.AddCheep(cheep);
    }
}