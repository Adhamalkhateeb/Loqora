
using FluentValidation;

namespace Loqora.Application.Features.Identity.Commands.RefreshTokens;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithErrorCode("RefreshToken:Required")
            .WithMessage("Refresh token is required.")
            .MaximumLength(512)
            .WithErrorCode("RefreshToken:TooLong")
            .WithMessage("Refresh token exceeds maximum length.");

        RuleFor(x => x.ExpiredAccessToken)
            .NotEmpty()
            .WithErrorCode("ExpiredAccessToken:Required")
            .WithMessage("Expired access token is required.");
    }
}
