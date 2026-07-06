using MediatR;
using Microsoft.Extensions.Logging;
using Loqora.Application.Common.Interfaces;

namespace Loqora.Application.Features.Identity.Events;

public sealed class PasswordResetEmailHandler(
    IEmailService emailService,
    IIdentityLinkGenerator linkGenerator,
    ILogger<PasswordResetEmailHandler> logger) : INotificationHandler<PasswordResetEmailEvent>
{
    private readonly IEmailService _emailService = emailService;
    private readonly IIdentityLinkGenerator _linkGenerator = linkGenerator;
    private readonly ILogger<PasswordResetEmailHandler> _logger = logger;

    public async Task Handle(PasswordResetEmailEvent notification, CancellationToken ct)
    {
        var resetLink = _linkGenerator.GenerateResetPasswordLink(
            notification.UserId,
            notification.ResetToken);

        var emailResult = await _emailService.SendPasswordResetAsync(
            notification.Email,
            notification.FullName,
            resetLink,
            ct);

        if (emailResult.IsError)
        {
            _logger.LogError(
                "Failed to send password reset email to user {UserId} ({Email}). {@Errors}",
                notification.UserId,
                UtilityService.MaskEmail(notification.Email),
                emailResult.Errors);
            return;
        }

        _logger.LogInformation(
            "Password reset email sent to user {UserId} ({Email}).",
            notification.UserId,
            UtilityService.MaskEmail(notification.Email));

    }
}
