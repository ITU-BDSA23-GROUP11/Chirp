using Chirp.Utilities.Models;

namespace Chirp.CSVDB
{
    public interface IDatabaseRepository
    {
        void AddCheep(Cheep cheep);
        
        List<Cheep> GetCheeps();
    }
}
