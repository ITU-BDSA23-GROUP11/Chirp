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
}