using System.Security.Claims;

namespace Chirp.WebService.Extensions;

public static class UserExtensions
{
    public static string GetUserFullName(this ClaimsPrincipal claims)
    {
        var givenNameClaim = claims.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname");
        var surnameClaim = claims.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname");
        
        return (givenNameClaim != null && surnameClaim != null) ? $"{givenNameClaim.Value} {surnameClaim.Value}" : "No Name Found";
    }
}