using MediatR;

namespace AuctionHouse.Application.Users.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<RefreshTokenResponseDto>
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}