namespace AuctionHouse.Application.Users.Commands.CreateUser;

using MediatR;

public class CreateUserCommand : IRequest<Unit>
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}