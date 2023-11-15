using System.Security.Claims;
using Chirp.Infrastructure.Contexts;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Chirp.WebService.Extensions;

public static class UserExtensions
{
    
    public static string GetUserFullName(this ClaimsPrincipal claims)
    {
        var givenNameClaim = claims.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);
        var surnameClaim = claims.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname);
        
        return (givenNameClaim != null && surnameClaim != null) ? $"{givenNameClaim.Value} {surnameClaim.Value}" : "No Name Found";
    }
    
    public static string GetUserEmail(this ClaimsPrincipal claims)
    {
        var emailClaim = claims.Claims.FirstOrDefault(y => y.Type == "emails");
        return (emailClaim != null) ? emailClaim.Value : "No Email";
    }
    
    public static List<string> GetUserCheepIds(this ClaimsPrincipal claims, string authorName)
    {
        var authorClaim = claims.Claims.FirstOrDefault(c => c.Type == "Author");
        if (authorClaim == null)
        {
            return new List<string>();
        }
        
        var cheepIds = new List<string>();

        using (var context = new ChirpDbContext())
        {
            cheepIds = context.Cheeps
                .Where(x => x.Author.Name == authorName)
                .Select(x => x.CheepId.ToString())
                .ToList();
            
        }
      
        return cheepIds;
    }

 
}