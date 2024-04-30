namespace AuctionHouse.Application.Users.Commands.CreateUser;

using AuctionHouse.Application.Common.Exceptions;
using AuctionHouse.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Unit>
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(UserManager<User> userManager, ILogger<CreateUserCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userManager.FindByNameAsync(request.Username) is not null)
            throw new UserRegistrationException($"Username {request.Username} is already in use");

        if (await _userManager.FindByEmailAsync(request.Email) is not null)
            throw new UserRegistrationException($"Email {request.Email} is already in use");

        var newUser = new User { UserName = request.Username, Email = request.Email };
        var createUserResult = await _userManager.CreateAsync(newUser, request.Password);

        if (!createUserResult.Succeeded)
        {
            throw new UserRegistrationException($"Could not create user [{request.Username}]", 
                createUserResult.Errors.Select(x => $"{x.Code}: {x.Description}").ToArray());
        }

        _logger.LogInformation("New user [{username}] created successfully", request.Username);

        return Unit.Value;
    }
}