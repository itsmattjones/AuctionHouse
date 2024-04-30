namespace AuctionHouse.WebAPI;

using AuctionHouse.Application.Common.Exceptions;
using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        var currentHttpContext = _contextAccessor.HttpContext;
        var currentUser = await _userManager.GetUserAsync(currentHttpContext?.User);

        if (currentUser == null)
            throw new CurrentUserException("User was not found");

        return await _userManager.GetUserAsync(currentHttpContext?.User);
    }

    /// <inheritdoc/>
    public string GetCurrentUserToken()
    {
        var authorizationHeader = _contextAccessor.HttpContext.Request.Headers?["Authorization"];

        if (authorizationHeader.HasValue)
            return authorizationHeader.ToString();
        else
            throw new CurrentUserException($"Invalid token for user [{_contextAccessor.HttpContext.User?.Identity?.Name}]");
    }
}