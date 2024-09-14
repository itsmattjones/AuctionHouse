namespace AuctionHouse.Application.Users.Commands.LoginUser;

using AuctionHouse.Domain.Common.Result;
using MediatR;

public class LoginUserCommand : IRequest<Result<TokenResponseDto>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}