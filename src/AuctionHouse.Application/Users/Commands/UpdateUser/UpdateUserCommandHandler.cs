namespace AuctionHouse.Application.Users.Commands.UpdateUser;

using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Domain.Common.Result;
using AuctionHouse.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly UserManager<User> _userManager;
    private readonly IDateTime _dateTime;

    public UpdateUserCommandHandler(ICurrentUserContext currentUserContext, 
        UserManager<User> userManager, IDateTime dateTime)
    {
        _currentUserContext = currentUserContext;
        _userManager = userManager;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _currentUserContext.GetCurrentUserContext();

        if (IsRequestPropertyAvailableForUpdate(request.Email, user.Email))
        {
            if (await _userManager.FindByEmailAsync(request.Email!) is not null)
                return Result.Failure(Error.Invalid, $"Email {request.Email} is already in use");

            user.Email = request.Email;
        }

        if (IsRequestPropertyAvailableForUpdate(request.Username, user.UserName))
        {
            if (await _userManager.FindByNameAsync(request.Username!) is not null)
                return Result.Failure(Error.Invalid, $"Username {request.Username} is already in use");

            user.UserName = request.Username;
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, request.Password);
        }

        if (IsRequestPropertyAvailableForUpdate(request.ProfileImageUrl, user.ProfileImageUrl))
        {
            user.ProfileImageUrl = request.ProfileImageUrl!;
        }

        user.RefreshTokenExpiry = _dateTime.Now;
        await _userManager.UpdateAsync(user);

        return Result.Success();
    }

    private static bool IsRequestPropertyAvailableForUpdate(string? requestProperty, string? currentProperty) =>
        !string.IsNullOrWhiteSpace(requestProperty) && !string.Equals(requestProperty, currentProperty, StringComparison.OrdinalIgnoreCase);
}