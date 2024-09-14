using System;
using AuctionHouse.Application.Users.Commands.CreateUser;
using AuctionHouse.Application.Users.Commands.LoginUser;
using AuctionHouse.Application.Users.Commands.RefreshToken;
using AuctionHouse.Application.Users.Commands.RevokeToken;
using AuctionHouse.Application.Users.Commands.UpdateUser;
using AuctionHouse.Application.Users.Queries.GetCurrentUser;
using AuctionHouse.Domain.Common.Result;
using AuctionHouse.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace AuctionHouse.WebAPI.Endpoints;

public static class UserEndponints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/user", async (ISender sender) =>
        {
            Result<CurrentUserDto> result = await sender.Send(new GetCurrentUserQuery());

            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
        });

        app.MapPost("api/user", async (ISender sender, [FromBody] CreateUserCommand command) =>
        {
            Result result = await sender.Send(command);

            return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
        });

        app.MapPut("api/user", async (ISender sender, [FromBody] UpdateUserCommand command) =>
        {
            Result result = await sender.Send(command);

            return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
        });

        app.MapPost("api/user/revoke", async (HttpContext context, ISender sender, [FromBody] RevokeTokenCommand command) =>
        {
            Result result = await sender.Send(command);

            if (!result.IsSuccess)
                return result.ToProblemDetails();

            context.Response.Cookies.Delete("refreshToken", new CookieOptions { HttpOnly = true });

            return Results.Ok();
        });

        app.MapPost("api/user/login", async (HttpContext context, ISender sender, [FromBody] LoginUserCommand command) =>
        {
            Result<TokenResponseDto> result = await sender.Send(command);

            if (!result.IsSuccess)
                return result.ToProblemDetails();

            context.Response.Cookies.Append("refreshToken", result.Value.RefreshToken, 
                new CookieOptions { HttpOnly = true, Expires = DateTime.UtcNow.AddDays(7) });

            return Results.Ok(result.Value);
        });

        app.MapPost("api/user/refresh", async (HttpContext context, ISender sender, [FromBody] RefreshTokenCommand command) =>
        {
            Result<RefreshTokenResponseDto> result = await sender.Send(command);

            if (!result.IsSuccess)
                return result.ToProblemDetails();

            context.Response.Cookies.Append("refreshToken", result.Value.RefreshToken, 
                new CookieOptions { HttpOnly = true, Expires = DateTime.UtcNow.AddDays(7) });

            return Results.Ok(result.Value);
        });
    }
}