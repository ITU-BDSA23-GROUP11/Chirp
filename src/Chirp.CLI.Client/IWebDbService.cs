namespace Chirp.CLI.Client;

public interface IWebDbService<T>
{
    public Task<T[]> Read();

    public Task Write(T cheep);
}