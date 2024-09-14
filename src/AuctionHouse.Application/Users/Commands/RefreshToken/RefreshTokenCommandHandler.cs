namespace AuctionHouse.Application.Users.Commands.RefreshToken;

using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Domain.Common.Result;
using AuctionHouse.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponseDto>>
{
    private readonly ILogger<RefreshTokenCommandHandler> _logger;
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IDateTime _dateTime;

    public RefreshTokenCommandHandler(ITokenService tokenService, UserManager<User> userManager, 
        IDateTime dateTime, ILogger<RefreshTokenCommandHandler> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _dateTime = dateTime;
        _logger = logger;
    }

    public async Task<Result<RefreshTokenResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.Token);
        var principalEmailClaim = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrWhiteSpace(principalEmailClaim))
            return Result.Failure<RefreshTokenResponseDto>(Error.Invalid, "Could not find user associated with the provided tokens.");

        var user = await _userManager.FindByEmailAsync(principalEmailClaim);

        if (user is null || user.RefreshToken != request.RefreshToken)
            return Result.Failure<RefreshTokenResponseDto>(Error.Invalid, "Could not find user associated with the provided tokens.");

        if (user.RefreshTokenExpiry <= _dateTime.Now)
            return Result.Failure<RefreshTokenResponseDto>(Error.Invalid, "The refresh token has expired");

        var accessToken = _tokenService.CreateToken(user);
        var refreshToken = _tokenService.CreateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = _dateTime.Now.AddDays(2);

        await _userManager.UpdateAsync(user);

        _logger.LogInformation("Refreshed token for user [{id}] ({username})", user.Id, user.UserName);

        return new RefreshTokenResponseDto(accessToken, refreshToken);
    }
}