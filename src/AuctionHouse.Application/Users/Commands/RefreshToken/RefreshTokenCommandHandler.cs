namespace AuctionHouse.Application.Users.Commands.RefreshToken;

using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AuctionHouse.Application.Common.Exceptions;
using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponseDto>
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

    public async Task<RefreshTokenResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.Token);
        var currentUser = await _userManager.FindByEmailAsync(principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value);

        if (currentUser is null || currentUser.RefreshToken != request.RefreshToken)
            throw new RefreshTokenException("Could not find user associated with the provided tokens.");

        if (currentUser.RefreshTokenExpiry <= _dateTime.Now)
            throw new RefreshTokenException("The refresh token has expired.");

        var accessToken = _tokenService.CreateToken(currentUser);
        var refreshToken = _tokenService.CreateRefreshToken();

        currentUser.RefreshToken = refreshToken;
        currentUser.RefreshTokenExpiry = _dateTime.Now.AddDays(2);

        await _userManager.UpdateAsync(currentUser);

        _logger.LogInformation("Refreshed token for user [{id}] ({username})", currentUser.Id, currentUser.UserName);

        return new RefreshTokenResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken
        };
    }
}