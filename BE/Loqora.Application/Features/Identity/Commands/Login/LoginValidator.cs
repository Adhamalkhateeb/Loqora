using FluentValidation;
using Loqora.Domain.Identity;

namespace Loqora.Application.Features.Identity.Commands.Login;

public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.")
            .MaximumLength(IdentityConstants.EmailMaxLength)
            .WithMessage($"Email must not exceed {IdentityConstants.EmailMaxLength} characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
