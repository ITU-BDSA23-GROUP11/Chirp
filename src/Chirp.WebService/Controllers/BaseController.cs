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
    public virtual Func<bool> IsUserAuthenticated { get; }
    public virtual Func<Guid?> GetUserId { get; }
    public virtual Func<string> GetUserName { get; }
    public virtual Func<string> GetUserLogin { get; }
    public virtual Func<string> GetUserAvatarUrl { get; }
    public virtual Func<string> GetPathUrl { get; }
    
    protected readonly IAuthorRepository AuthorRepository;
    protected readonly ICheepRepository CheepRepository;
    protected readonly ILikeRepository LikeRepository;
    
    protected BaseController(IAuthorRepository authorRepository, ICheepRepository cheepRepository, ILikeRepository likeRepository)
    {
        AuthorRepository = authorRepository;
        CheepRepository = cheepRepository;
        LikeRepository = likeRepository;
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
        GetUserId = () => User.GetUserId();
        GetUserLogin = () => User.GetUserLogin();
        GetUserName = () => User.GetUserName() ?? GetUserLogin();
        GetUserAvatarUrl = () => User.GetUserAvatar();
        GetPathUrl = () => Request.GetPathUrl();
    }

    protected ActionResult WithAuth(Func<ClientUser, ActionResult> protectedFunction)
    {
        try
        {
            if (!IsUserAuthenticated()) return Unauthorized();
            
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            
            var user = new ClientUser
            {
                IsAuthenticated = IsUserAuthenticated(),
                Name = GetUserName(),
                Login = GetUserLogin(),
                AvatarUrl = GetUserAvatarUrl(),
                Id = userId.Value
            };
            
            AuthorRepository.AddAuthor(new AuthorDto
            {
                Id = user.Id,
                Name = user.Name,
                Login = user.Login,
                AvatarUrl = user.AvatarUrl
            });

            return protectedFunction(user);
        }
        catch
        {
            return BadRequest("Unknown Error Occurred");
        }
    }
}