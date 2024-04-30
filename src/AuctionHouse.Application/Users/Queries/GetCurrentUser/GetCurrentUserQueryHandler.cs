namespace AuctionHouse.Application.Users.Queries.GetCurrentUser;

using AuctionHouse.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IMapper _mapper;

    public GetCurrentUserQueryHandler(ICurrentUserContext currentUserContext, IMapper mapper)
    {
        _currentUserContext = currentUserContext;
        _mapper = mapper;
    }

    public async Task<CurrentUserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var currentUser = await _currentUserContext.GetCurrentUserContext();

        return _mapper.Map<CurrentUserDto>(currentUser);
    }
}