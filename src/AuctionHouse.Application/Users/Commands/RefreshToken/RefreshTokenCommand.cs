namespace AuctionHouse.Application.Users.Commands.RefreshToken;

using MediatR;

public class RefreshTokenCommand : IRequest<RefreshTokenResponseDto>
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}