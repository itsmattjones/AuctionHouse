using MediatR;

namespace AuctionHouse.Application.Users.Commands.LoginUser
{
    public class LoginUserCommand : IRequest<TokenResponseDto>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}