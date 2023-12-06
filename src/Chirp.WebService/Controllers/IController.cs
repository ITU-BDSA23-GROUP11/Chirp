namespace Chirp.WebService.Controllers;

public interface IController
{
    public Func<bool> IsUserAuthenticated { get; }
    public Func<Guid?> GetUserId { get; }
    public Func<string> GetUserName { get; }
    public Func<string> GetUserLogin { get; }
    public Func<string> GetUserAvatarUrl { get; }
    public Func<string> GetPathUrl { get; }
}