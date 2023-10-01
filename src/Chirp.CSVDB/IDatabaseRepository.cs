namespace Chirp.CSVDB
{
    public interface IDatabaseRepository
    {
        void AddCheep(Cheep cheep);
        
        List<string> GetCheeps();
    }
}
