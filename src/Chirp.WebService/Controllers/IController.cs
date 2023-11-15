namespace Chirp.WebService.Controllers;

public interface IController
{
    public Func<bool> IsUserAuthenticated { get; }
    public Func<string> GetUserFullName { get; }
    public Func<string> GetUserEmail { get; }
    public Func<string> GetPathUrl { get; }
}