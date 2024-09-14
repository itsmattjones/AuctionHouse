namespace AuctionHouse.Application.Users.Commands.RevokeToken;

using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Domain.Common.Result;
using AuctionHouse.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, Result>
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly UserManager<User> _userManager;
    private readonly IDateTime _dateTime;

    public RevokeTokenCommandHandler(ICurrentUserContext currentUserContext,
        UserManager<User> userManager, IDateTime dateTime)
    {
        _currentUserContext = currentUserContext;
        _userManager = userManager;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _currentUserContext.GetCurrentUserContext();

        user.RefreshTokenExpiry = _dateTime.Now;

        await _userManager.UpdateAsync(user);

        return Result.Success();
    }
}