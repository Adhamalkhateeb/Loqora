using FluentValidation;
using Loqora.Domain.Identity;

namespace Loqora.Application.Features.Identity.Commands.RegisterGuest;

public sealed class RegisterGuestValidator : AbstractValidator<RegisterGuestCommand>
{
    public RegisterGuestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(IdentityConstants.FirstNameMaxLength)
            .WithMessage($"First name must not exceed {IdentityConstants.FirstNameMaxLength} characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(IdentityConstants.LastNameMaxLength)
            .WithMessage($"Last name must not exceed {IdentityConstants.LastNameMaxLength} characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.")
            .MaximumLength(IdentityConstants.EmailMaxLength)
            .WithMessage($"Email must not exceed {IdentityConstants.EmailMaxLength} characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(IdentityConstants.PasswordMinLength)
            .WithMessage($"Password must be at least {IdentityConstants.PasswordMinLength} characters.")
            .MaximumLength(IdentityConstants.PasswordMaxLength)
            .WithMessage($"Password must not exceed {IdentityConstants.PasswordMaxLength} characters.")
            .Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{8,30}$")
            .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character.");
    }
}
