namespace AuctionHouse.Application.Users.Commands.UpdateUser;

using AuctionHouse.Domain.Common.Result;
using MediatR;

public class UpdateUserCommand : IRequest<Result>
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string ProfileImageUrl { get; set; }
}