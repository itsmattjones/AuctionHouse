namespace AuctionHouse.Application.Users.Commands.CreateUser;

using AuctionHouse.Domain.Common.Result;
using AuctionHouse.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result>
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(UserManager<User> userManager, ILogger<CreateUserCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userManager.FindByNameAsync(request.Username) is not null)
            return Result.Failure(Error.Conflict, $"Username {request.Username} is already in use.");

        if (await _userManager.FindByEmailAsync(request.Email) is not null)
            return Result.Failure(Error.Conflict, $"Email {request.Email} is already in use.");

        var user = new User { UserName = request.Username, Email = request.Email };
        var createUserResult = await _userManager.CreateAsync(user, request.Password);

        if (!createUserResult.Succeeded)
        {
            _logger.LogError("Could not create user [{username}] Errors: {errors}", request.Username,
                createUserResult.Errors.Select(x => $"{x.Code}: {x.Description}").ToArray());

            return Result.Failure(Error.Critial, $"Could not create user [{request.Username}]");
        }

        _logger.LogInformation("New user [{username}] created successfully", request.Username);

        return Result.Success();
    }
}