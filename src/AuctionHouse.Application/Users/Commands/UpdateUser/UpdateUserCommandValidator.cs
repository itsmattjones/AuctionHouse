using FluentValidation;

namespace AuctionHouse.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(u => u.Email)
                .EmailAddress();
        }
    }
}