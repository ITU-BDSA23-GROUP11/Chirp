using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Chirp.WebService.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.WebService.Controllers;

public struct ClientUser
{
    public required bool IsAuthenticated { get; set; }
    public Guid Id { get; init; }
    public string FullName { get; init; }
    public string Email { get; init; }
}

public abstract class BaseController : Controller, IController
{
    public virtual Func<bool> IsUserAuthenticated { get; }
    public virtual Func<Guid?> GetUserId { get; }
    public virtual Func<string> GetUserFullName { get; }
    public virtual Func<string> GetUserEmail { get; }
    public virtual Func<string> GetPathUrl { get; }
    
    protected readonly IAuthorRepository AuthorRepository;
    protected readonly ICheepRepository CheepRepository;
    
    protected BaseController(IAuthorRepository authorRepository, ICheepRepository cheepRepository)
    {
        AuthorRepository = authorRepository;
        CheepRepository = cheepRepository;
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
        GetUserEmail = () => User.GetUserEmail();
        GetUserFullName = () => User.GetUserFullName();
        GetPathUrl = () => Request.GetPathUrl();
    }

    protected ActionResult WithAuth(Func<ClientUser, ActionResult> protectedFunction)
    {
        try
        {
            if (IsUserAuthenticated())
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                var user = new ClientUser
                {
                    IsAuthenticated = IsUserAuthenticated(),
                    FullName = GetUserFullName(),
                    Email = GetUserEmail(),
                    Id = userId ?? new Guid()
                };

                AuthorRepository.AddAuthor(new AuthorDto
                {
                    Id = user.Id,
                    Name = user.FullName,
                    Email = user.Email
                });

                return protectedFunction(user);
            }

            return Unauthorized();
        }
        catch
        {
            return BadRequest("Unknown Error Occurred");
        }
    }
}