namespace SimpleDB;

public class CSVDatabase<T>: IDatabaseRepository<T>
{
    public IEnumerable<T> Read(int? limit = null)
    {
        return null;
    }

    public void Store(T record)
    {
        
    }
}