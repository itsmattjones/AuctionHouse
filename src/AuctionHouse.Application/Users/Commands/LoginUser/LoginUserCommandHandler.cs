namespace AuctionHouse.Application.Users.Commands.LoginUser;

using System.Threading;
using System.Threading.Tasks;
using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Domain.Common.Result;
using AuctionHouse.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<TokenResponseDto>>
{
    private readonly ILogger<LoginUserCommandHandler> _logger;
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IDateTime _dateTime;

    public LoginUserCommandHandler(UserManager<User> userManager, ITokenService tokenService, 
        IDateTime dateTime, ILogger<LoginUserCommandHandler> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _dateTime = dateTime;
        _logger = logger;
    }

    public async Task<Result<TokenResponseDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userManager.FindByEmailAsync(request.Email.ToUpperInvariant()) is var user && user is null)
            return Result.Failure<TokenResponseDto>(Error.Invalid);

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            return Result.Failure<TokenResponseDto>(Error.Invalid);

        var accessToken = _tokenService.CreateToken(user);
        var refreshToken = _tokenService.CreateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = _dateTime.Now.AddDays(2);

        await _userManager.UpdateAsync(user);

        _logger.LogInformation("Login successful for user [{id}] ({username})", user.Id, user.UserName);

        return new TokenResponseDto(accessToken, refreshToken);
    }
}