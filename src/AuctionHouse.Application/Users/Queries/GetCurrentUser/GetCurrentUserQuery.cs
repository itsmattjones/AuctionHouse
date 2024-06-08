namespace AuctionHouse.Application.Users.Queries.GetCurrentUser;

using AuctionHouse.Domain.Common.Result;
using MediatR;

public class GetCurrentUserQuery : IRequest<Result<CurrentUserDto>>
{
}