using Chirp.Core.Dto;
using Chirp.Core.Extensions;
using Chirp.Core.Repositories;
using Chirp.WebService.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.WebService.Controllers;

public abstract class BaseController : Controller, IController
{
    public virtual Func<ClaimsUser?> GetUser { get; }
    public virtual Func<string> GetPathUrl { get; }
    
    protected readonly IAuthorRepository AuthorRepository;
    protected readonly ICheepRepository CheepRepository;
    protected readonly ILikeRepository LikeRepository;
    protected readonly ICommentRepository CommentRepository;
    
    protected BaseController(IAuthorRepository authorRepository, ICheepRepository cheepRepository, ILikeRepository likeRepository, ICommentRepository commentRepository)
    {
        AuthorRepository = authorRepository;
        CheepRepository = cheepRepository;
        CommentRepository = commentRepository;
        
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
                Username = user.Username,
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

    public ActionResult RedirectWithError(string errorMessage)
    {
        var pathUrl = GetPathUrl();
        if (pathUrl.Contains("errorMessage=")) return Redirect(pathUrl);
        
        var queryDelimiter = pathUrl.Contains('?') ? "&" : "?";
        return Redirect(GetPathUrl() + $"{queryDelimiter}errorMessage={errorMessage}");
    }
}