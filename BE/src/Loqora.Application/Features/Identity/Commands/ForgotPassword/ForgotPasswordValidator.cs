using FluentValidation;
using Loqora.Domain.Identity;

namespace Loqora.Application.Features.Identity.Commands.ForgotPassword;

public sealed class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.")
            .MaximumLength(IdentityConstants.EmailMaxLength)
            .WithMessage($"Email must not exceed {IdentityConstants.EmailMaxLength} characters.");
    }
}
