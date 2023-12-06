using System.Security.Claims;

namespace Chirp.WebService.Extensions;

public static class UserExtensions
{
    
    public static string? GetUserName(this ClaimsPrincipal claims)
    {
        var name = claims.Claims.FirstOrDefault(x => x.Type == "name");

        return name?.Value ?? null;
    }
    
    public static string GetUserAvatar(this ClaimsPrincipal claims)
    {
        // IEF with github guarantees avatar url
        return claims.Claims.FirstOrDefault(x => x.Type == "avatar_url")!.Value;
    }

    public static string GetUserLogin(this ClaimsPrincipal claims)
    {
        // IEF with github guarantees user login
        return claims.Claims.FirstOrDefault(x => x.Type == "login")!.Value;
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