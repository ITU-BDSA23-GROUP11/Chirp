using Chirp.Core.Extensions;

namespace Chirp.WebService.Controllers;

public interface IController
{
    public Func<ClaimsUser?> GetUser { get; }
    public Func<string> GetPathUrl { get; }
}