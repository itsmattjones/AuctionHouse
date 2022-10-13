using FluentValidation;

namespace AuctionHouse.Application.Users.Commands.CreateUser
{

    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(u => u.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(u => u.Username)
                .NotEmpty();

            RuleFor(u => u.Password)
                .NotEmpty();
        }
    }
}