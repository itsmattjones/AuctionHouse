namespace AuctionHouse.Application.Users.Commands.CreateUser;

using AuctionHouse.Domain.Common.Result;
using MediatR;

public class CreateUserCommand : IRequest<Result>
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}