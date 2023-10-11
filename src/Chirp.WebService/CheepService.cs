public record CheepViewModel(string Author, string Text, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    // These would normally be loaded from a database for example
    private static readonly List<CheepViewModel> _cheeps = new()
    {
        new CheepViewModel("Helge", "Hello, BDSA students!", UnixTimestampToDateTimeString(1690892208)),
        new CheepViewModel("Rasmus", "Hej, velkommen til kurset.", UnixTimestampToDateTimeString(1690895308)),
    };

    public List<CheepViewModel> GetCheeps()
    {
        return _cheeps;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        // filter by the provided author name
        return _cheeps.Where(x => x.Author == author).ToList();
    }

    private static string UnixTimestampToDateTimeString(double unixTimestamp)
    {
        // Unix Timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimestamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}