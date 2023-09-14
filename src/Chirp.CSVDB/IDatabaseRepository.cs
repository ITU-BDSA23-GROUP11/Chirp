namespace Chirp.CSVDB;

interface IDatabaseRepository<T>
{
    public IEnumerable<T> Read(string filePath);
    public void Store(T record);
}