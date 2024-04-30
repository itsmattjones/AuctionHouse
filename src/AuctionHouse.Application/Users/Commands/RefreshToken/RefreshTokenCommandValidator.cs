namespace AuctionHouse.Application.Users.Commands.RefreshToken;

using FluentValidation;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(t => t.Token)
            .NotEmpty();

        RuleFor(t => t.RefreshToken)
            .NotEmpty();
    }
}