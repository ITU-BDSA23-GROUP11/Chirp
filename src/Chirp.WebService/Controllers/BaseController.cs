using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Repositories;
using Chirp.WebService.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.WebService.Controllers;

public struct ClientUser
{
    public required bool IsAuthenticated;
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Login { get; init; }
    public string AvatarUrl { get; init; }
}

public abstract class BaseController : Controller, IController
{
    public virtual Func<ClaimsUser?> GetUser { get; }
    public virtual Func<string> GetPathUrl { get; }
    
    protected readonly IAuthorRepository AuthorRepository;
    protected readonly ICheepRepository CheepRepository;
    protected readonly ILikeRepository LikeRepository;
    
    protected BaseController(IAuthorRepository authorRepository, ICheepRepository cheepRepository, ILikeRepository likeRepository)
    {
        AuthorRepository = authorRepository;
        CheepRepository = cheepRepository;
        
        GetUser = () => User.GetUser();
        LikeRepository = likeRepository;
        GetUser = () => User.GetUser();
        GetPathUrl = () => Request.GetPathUrl();
    }

    protected ActionResult WithAuth(Func<ClaimsUser, ActionResult> protectedFunction)
    {
        try
        {
            var user = GetUser().ThrowIfNull();
            AuthorRepository.AddAuthor(new AuthorDto
            {
                Id = user.Id,
                Name = user.Name,
                Login = user.Login,
                AvatarUrl = user.AvatarUrl
            });

            return protectedFunction(user);
        }
        catch (ArgumentException)
        {
            return Unauthorized();
        }
        catch
        {
            return BadRequest("Unknown Error Occurred");
        }
    }
}