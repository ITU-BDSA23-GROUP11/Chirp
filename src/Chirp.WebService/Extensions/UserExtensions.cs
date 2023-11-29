using System.Security.Claims;

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

    public static Guid? GetUserId(this ClaimsPrincipal claims)
    {
        var idClaim = claims.Claims.FirstOrDefault(y => y.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier");
        return idClaim == null ? null : Guid.Parse(idClaim.Value);
    }
}