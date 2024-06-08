namespace AuctionHouse.Application.Users.Queries.GetCurrentUser;

using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Domain.Common.Result;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<CurrentUserDto>>
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IMapper _mapper;

    public GetCurrentUserQueryHandler(ICurrentUserContext currentUserContext, IMapper mapper)
    {
        _currentUserContext = currentUserContext;
        _mapper = mapper;
    }

    public async Task<Result<CurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var currentUser = await _currentUserContext.GetCurrentUserContext();

        return _mapper.Map<CurrentUserDto>(currentUser);
    }
}