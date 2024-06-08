namespace AuctionHouse.Application.Users.Commands.CreateUser;

using AuctionHouse.Domain.Common.Result;
using MediatR;

public class CreateUserCommand : IRequest<Result>
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}