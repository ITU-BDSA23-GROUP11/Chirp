using Chirp.WebService.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.WebService.Controllers;


public abstract class BaseController : Controller, IController
{
    public virtual Func<bool> IsUserAuthenticated { get; }
    public virtual Func<string> GetUserFullName { get; }
    public virtual Func<string> GetUserEmail { get; }
    public virtual Func<string> GetPathUrl { get; }
    
    protected BaseController()
    {
        IsUserAuthenticated = () =>
        {
            try
            {
                return User.Identity?.IsAuthenticated ?? false;
            }
            catch
            {
                return false;
            }
        };
        GetUserEmail = () => User.GetUserEmail();
        GetUserFullName = () => User.GetUserFullName();
        GetPathUrl = () => Request.GetPathUrl();
    }

    protected ActionResult WithAuth(Func<ActionResult> protectedFunction)
    {
        if (IsUserAuthenticated())
        {
            return protectedFunction();
        }
        return Unauthorized();
    }
}