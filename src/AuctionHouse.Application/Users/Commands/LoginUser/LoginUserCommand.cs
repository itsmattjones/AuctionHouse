namespace AuctionHouse.Application.Users.Commands.LoginUser;

using MediatR;

public class LoginUserCommand : IRequest<TokenResponseDto>
{
    public string Email { get; set; }
    public string Password { get; set; }
}