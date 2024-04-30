namespace AuctionHouse.WebAPI.Controllers;

using AuctionHouse.Application.Common.Exceptions;
using AuctionHouse.Application.Users.Commands.CreateUser;
using AuctionHouse.Application.Users.Commands.LoginUser;
using AuctionHouse.Application.Users.Commands.RefreshToken;
using AuctionHouse.Application.Users.Commands.RevokeToken;
using AuctionHouse.Application.Users.Commands.UpdateUser;
using AuctionHouse.Application.Users.Queries.GetCurrentUser;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

[Authorize]
public class UserController : ApiControllerBase
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CurrentUserDto), StatusCodes.Status200OK)]
    public async Task<CurrentUserDto> RetrieveCurrentUser()
    {
        _logger.LogInformation("Retrieving current user");
        return await Mediator.Send(new GetCurrentUserQuery());
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] CreateUserCommand command)
    {
        _logger.LogInformation("Creating user for request [{email}]", command.Email);

        try
        {
            return Ok(await Mediator.Send(command));
        }
        catch (Exception ex) when (ex is UserRegistrationException || ex is ValidationException)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserCommand command)
    {
        _logger.LogInformation("Update user [{name}]", HttpContext.User.Identity?.Name ?? "unknown");

        try
        {
            return Ok(await Mediator.Send(command));
        }
        catch (Exception ex) when (ex is UserUpdateException || ex is ValidationException)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost(nameof(Revoke))]
    public async Task<IActionResult> Revoke([FromBody] RevokeTokenCommand command)
    {
        _logger.LogInformation("Revoking refresh token for user [{name}]", HttpContext.User.Identity?.Name ?? "unknown");

        try
        {
            var response = await Mediator.Send(command);

            Response.Cookies.Delete("refreshToken", new CookieOptions { HttpOnly = true });

            return Ok(response);
        }
        catch (RefreshTokenException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost(nameof(Login))]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        _logger.LogInformation("Attempting login request for user [{email}]", command.Email);

        try
        {
            var response = await Mediator.Send(command);

            Response.Cookies.Append("refreshToken", response.RefreshToken,
                new CookieOptions { HttpOnly = true, Expires = DateTime.UtcNow.AddDays(7) });

            return Ok(response);
        }
        catch (Exception ex) when (ex is UserLoginException || ex is ValidationException)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost(nameof(Refresh))]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
    {
        _logger.LogInformation("Refreshing token for user [{name}]", HttpContext.User.Identity?.Name ?? "unknown");

        try
        {
            var response = await Mediator.Send(command);

            Response.Cookies.Append("refreshToken", response.RefreshToken,
                new CookieOptions { HttpOnly = true, Expires = DateTime.UtcNow.AddDays(7) });

            return Ok(response);
        }
        catch (Exception ex) when (ex is RefreshTokenException || ex is ValidationException)
        {
            return BadRequest(ex.Message);
        }
    }
}