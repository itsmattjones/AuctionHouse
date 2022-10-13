using System.Threading;
using System.Threading.Tasks;
using AuctionHouse.Application.Common.Exceptions;
using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuctionHouse.Application.Users.Commands.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, TokenResponseDto>
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

        public async Task<TokenResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            if (await _userManager.FindByEmailAsync(request.Email.ToUpperInvariant()) is var existingUser && existingUser is null)
                throw new UserLoginException();

            if (!await _userManager.CheckPasswordAsync(existingUser, request.Password))
                throw new UserLoginException();

            var accessToken = _tokenService.CreateToken(existingUser);
            var refreshToken = _tokenService.CreateRefreshToken();

            existingUser.RefreshToken = refreshToken;
            existingUser.RefreshTokenExpiry = _dateTime.Now.AddDays(2);

            await _userManager.UpdateAsync(existingUser);

            _logger.LogInformation("Login successful for user [{id}] ({username})", existingUser.Id, existingUser.UserName);

            return new TokenResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}