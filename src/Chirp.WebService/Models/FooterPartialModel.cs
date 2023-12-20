namespace Chirp.WebService.Models;

public struct FooterPartialModel
{
    public string? FirstLink;
    
    public string? PreviousLink;
    public int? PreviousPage;
    
    public int CurrentPage;
    
    public string? NextLink;
    public int? NextPage;
    
    public string? LastLink;
    public int? LastPage;

    public static FooterPartialModel BuildFooterPartialModel(
        int pageNumber,
        int amountOfPages,
        string author = ""
    )
    {
        return new FooterPartialModel
        {
            FirstLink = pageNumber > 2 ? $"/{author}?page=1" : null,
            
            PreviousLink = pageNumber > 1 ? $"/{author}?page={pageNumber-1}" : null,
            PreviousPage = pageNumber > 1 ? pageNumber-1 : null,
            
            CurrentPage = pageNumber,
            
            NextLink = pageNumber < amountOfPages ? $"/{author}?page={pageNumber+1}" : null,
            NextPage = pageNumber < amountOfPages ? pageNumber+1 : null,
            
            LastLink = pageNumber < amountOfPages-1 ? $"/{author}?page={amountOfPages}" : null,
            LastPage = pageNumber < amountOfPages-1 ? amountOfPages : null,
        };
    }
}