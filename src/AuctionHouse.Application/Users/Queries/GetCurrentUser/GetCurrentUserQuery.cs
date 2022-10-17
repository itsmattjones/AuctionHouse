using MediatR;

namespace AuctionHouse.Application.Users.Queries.GetCurrentUser
{
    public class GetCurrentUserQuery : IRequest<CurrentUserDto>
    {
    }
}