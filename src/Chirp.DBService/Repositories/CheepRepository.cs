using Chirp.DBService.Models;

namespace Chirp.DBService.Repositories;

public class CheepRepository : ICheepRepository
{
    public CheepRepository()
    {
        DbInitializer.SeedDatabase(ChirpDBContext.GetInstance());
    }
    
    public Cheep AddCheep(Cheep cheep)
    {
        ChirpDBContext.GetInstance().Cheeps.Add(cheep);
        ChirpDBContext.GetInstance().SaveChanges();
        return cheep;
    }

    public List<Cheep> GetCheeps()
    {
        return ChirpDBContext.GetInstance().Cheeps.ToList();
    }
    
    public List<Cheep> GetCheepsForPage(int pageNumber)
    {
       return ChirpDBContext.GetInstance().Cheeps.Skip(pageNumber * 32).Take(32).ToList();
    }

    public int GetCheepCount()
    {
        return ChirpDBContext.GetInstance().Cheeps.Count();
    }

    public List<Cheep> GetCheepsFromAuthorName(string authorName)
    {
        return ChirpDBContext.GetInstance().Cheeps.Where(c => c.Author.Name == authorName).ToList();
    }
}