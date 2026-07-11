using Loqora.Application.Common.Interfaces;
using Loqora.Application.Features.Identity.Events;
using Loqora.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Loqora.Application.Features.Identity.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    IIdentityService identityService,
    IPublisher publisher,
    ILogger<ForgotPasswordCommandHandler> logger) : IRequestHandler<ForgotPasswordCommand, Result<Success>>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IPublisher _publisher = publisher;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger = logger;

    public async Task<Result<Success>> Handle(ForgotPasswordCommand request, CancellationToken ct)
    {

        var result = await _identityService.GeneratePasswordResetTokenAsync(request.Email, ct);

        if (result.IsError)
        {
            _logger.LogWarning("Failed to generate password reset token for {Email} {@Errors}.",
                UtilityService.MaskEmail(request.Email),
                result.Errors);
            return Result.Success; // Return success to avoid revealing whether the email exists
        }

        await _publisher.Publish(
            new PasswordResetEmailEvent(
                result.Value.UserId,
                request.Email,
                result.Value.UserFullName,
                result.Value.Token),
            ct);

        return Result.Success;
    }
}
