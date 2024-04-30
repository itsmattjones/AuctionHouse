namespace AuctionHouse.Application.Users.Commands.UpdateUser;

using MediatR;

public class UpdateUserCommand : IRequest<Unit>
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string ProfileImageUrl { get; set; }
}