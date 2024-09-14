namespace AuctionHouse.WebAPI;

using AuctionHouse.Application.Common.Exceptions;
using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

public class CurrentUserContext : ICurrentUserContext
{
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    /// <summary>
    /// Creates a new instance of the <see cref="CurrentUserContext"/> class.
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="contextAccessor"></param>
    public CurrentUserContext(UserManager<User> userManager, IHttpContextAccessor contextAccessor)
    {
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }

    /// <inheritdoc/>
    public async Task<User> GetCurrentUserContext()
    {
        var currentHttpContext = _contextAccessor.HttpContext ??
            throw new CurrentUserException("Could not access HttpContext");

        if (await _userManager.GetUserAsync(currentHttpContext.User) is var user && user is null)
            throw new CurrentUserException("The current user was not found");

        return user;
    }

    /// <inheritdoc/>
    public string GetCurrentUserToken()
    {
        var currentHttpContext = _contextAccessor.HttpContext ??
            throw new CurrentUserException("Could not access HttpContext");

        var authorizationHeader = currentHttpContext.Request.Headers.Authorization.SingleOrDefault() ??
            throw new CurrentUserException($"Invalid token for user [{currentHttpContext.User?.Identity?.Name}]");

        return authorizationHeader;
    }
}