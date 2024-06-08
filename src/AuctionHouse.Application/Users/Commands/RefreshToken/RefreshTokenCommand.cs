namespace AuctionHouse.Application.Users.Commands.RefreshToken;

using AuctionHouse.Domain.Common.Result;
using MediatR;

public class RefreshTokenCommand : IRequest<Result<RefreshTokenResponseDto>>
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}