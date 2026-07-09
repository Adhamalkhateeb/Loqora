using FluentValidation;
using Loqora.Domain.Identity;

namespace Loqora.Application.Features.Identity.Commands.ResetPassword;

public sealed class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.")
            .MaximumLength(IdentityConstants.EmailMaxLength)
            .WithMessage($"Email must not exceed {IdentityConstants.EmailMaxLength} characters.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Reset token is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(IdentityConstants.PasswordMinLength)
            .WithMessage($"Password must be at least {IdentityConstants.PasswordMinLength} characters.")
            .MaximumLength(IdentityConstants.PasswordMaxLength)
            .WithMessage($"Password must not exceed {IdentityConstants.PasswordMaxLength} characters.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,30}$")
            .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character.");
    }
}
