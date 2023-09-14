using System.Collections;

namespace Chirp.CSVDB;

public class CSVDatabase<T>: IDatabaseRepository<T>
{
    public IEnumerable<T> Read(string filePath)
    {
        return null;
    }

    public void Store(T record)
    {
        
    }
}