using System.Security.Claims;

namespace Chirp.WebService.Extensions;

public struct ClaimsUser
{
    public string Name { get; init; }
    public string Username { get; init; }
    public string AvatarUrl { get; init; }
    public Guid Id { get; init; }
}

public static class UserExtensions
{
    public static ClaimsUser? GetUser(this ClaimsPrincipal claims)
    {
        var name = claims.Claims.FirstOrDefault(x => x.Type == "name");
        var avatarUrl = claims.Claims.FirstOrDefault(x => x.Type == "avatar_url");
        var username = claims.Claims.FirstOrDefault(x => x.Type == "login");
        var id = claims.Claims.FirstOrDefault(y =>
            y.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        
        if (avatarUrl == null || username == null || id == null) return null;

        return new ClaimsUser
        {
            Name = name?.Value ?? username.Value,
            Username = username.Value,
            AvatarUrl = avatarUrl.Value,
            Id = Guid.Parse(id.Value)
        };
    }
}