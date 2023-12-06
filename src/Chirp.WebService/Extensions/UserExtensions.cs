using System.Security.Claims;

namespace Chirp.WebService.Extensions;

public struct ClaimsUser
{
    public string Name { get; init; }
    public string Login { get; init; }
    public string AvatarUrl { get; init; }
    public Guid Id { get; init; }
}

public static class UserExtensions
{
    public static ClaimsUser? GetUser(this ClaimsPrincipal claims)
    {
        var name = claims.Claims.FirstOrDefault(x => x.Type == "name");
        var avatarUrl = claims.Claims.FirstOrDefault(x => x.Type == "avatar_url");
        var login = claims.Claims.FirstOrDefault(x => x.Type == "login");
        var id = claims.Claims.FirstOrDefault(y =>
            y.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        
        if (avatarUrl == null || login == null || id == null) return null;

        return new ClaimsUser
        {
            Name = name?.Value ?? login.Value,
            Login = login.Value,
            AvatarUrl = avatarUrl.Value,
            Id = Guid.Parse(id.Value)
        };
    }
}