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
        var currentUser = await _currentUserContext.GetCurrentUserContext();

        if (IsRequestPropertyAvailableForUpdate(request.Email, currentUser.Email))
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not null)
                return Result.Failure(Error.Invalid, $"Email {request.Email} is already in use");

            currentUser.Email = request.Email;
        }

        if (IsRequestPropertyAvailableForUpdate(request.Username, currentUser.UserName))
        {
            if (await _userManager.FindByNameAsync(request.Username) is not null)
                return Result.Failure(Error.Invalid, $"Username {request.Username} is already in use");

            currentUser.UserName = request.Username;
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            await _userManager.RemovePasswordAsync(currentUser);
            await _userManager.AddPasswordAsync(currentUser, request.Password);
        }

        if (IsRequestPropertyAvailableForUpdate(request.ProfileImageUrl, currentUser.ProfileImageUrl))
            currentUser.ProfileImageUrl = request.ProfileImageUrl;

        currentUser.RefreshTokenExpiry = _dateTime.Now;
        await _userManager.UpdateAsync(currentUser);

        return Result.Success();
    }

    private static bool IsRequestPropertyAvailableForUpdate(string requestProperty, string currentProperty)
    {
        return !string.IsNullOrWhiteSpace(requestProperty)  
            && !string.Equals(requestProperty, currentProperty, StringComparison.OrdinalIgnoreCase);
    }
}