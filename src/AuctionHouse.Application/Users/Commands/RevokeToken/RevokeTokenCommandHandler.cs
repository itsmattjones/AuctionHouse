using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionHouse.Application.Users.Commands.RevokeToken
{
    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, Unit>
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

        public async Task<Unit> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _currentUserContext.GetCurrentUserContext();

            currentUser.RefreshTokenExpiry = _dateTime.Now;

            await _userManager.UpdateAsync(currentUser);

            return Unit.Value;
        }
    }
}