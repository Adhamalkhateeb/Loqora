using MediatR;
using Microsoft.Extensions.Logging;
using Loqora.Application.Common.Interfaces;

namespace Loqora.Application.Features.Identity.Events;

public sealed class GuestConfirmationEmailHandler(
    IIdentityService identityService,
    IEmailService emailService,
    IIdentityLinkGenerator linkGenerator,
    ILogger<GuestConfirmationEmailHandler> logger) : INotificationHandler<GuestConfirmationEmailEvent>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IEmailService _emailService = emailService;
    private readonly IIdentityLinkGenerator _linkGenerator = linkGenerator;
    private readonly ILogger<GuestConfirmationEmailHandler> _logger = logger;

    public async Task Handle(GuestConfirmationEmailEvent notification, CancellationToken ct)
    {

        var tokenResult = await _identityService.GenerateEmailConfirmationTokenAsync(notification.UserId);

        if (tokenResult.IsError)
        {
            _logger.LogError(
                "Failed to generate email confirmation token for user {UserId}. {@Errors}",
                notification.UserId,
                tokenResult.Errors);
            return;
        }

        var confirmationLink = _linkGenerator.GenerateEmailConfirmationLink(
            notification.UserId,
            tokenResult.Value);

        var emailResult = await _emailService.SendEmailConfirmationAsync(
            notification.Email,
            notification.FullName,
            confirmationLink,
            ct);

        if (emailResult.IsError)
        {
            _logger.LogError(
                "Failed to send confirmation email to user {UserId} ({Email}). {@Errors}",
                notification.UserId,
                UtilityService.MaskEmail(notification.Email),
                emailResult.Errors);
            return;
        }

        _logger.LogInformation(
            "Confirmation email sent to user {UserId} ({Email}).",
            notification.UserId,
            UtilityService.MaskEmail(notification.Email));
    }
}
