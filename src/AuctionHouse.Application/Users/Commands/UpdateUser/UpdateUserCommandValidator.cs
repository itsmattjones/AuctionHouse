namespace AuctionHouse.Application.Users.Commands.UpdateUser;

using FluentValidation;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(u => u.Email)
            .EmailAddress();
    }
}