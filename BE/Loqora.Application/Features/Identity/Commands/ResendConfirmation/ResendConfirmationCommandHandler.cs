using MediatR;
using Microsoft.Extensions.Logging;
using Loqora.Application.Common.Interfaces;
using Loqora.Application.Features.Identity.Events;
using Loqora.Domain.Common.Results;

namespace Loqora.Application.Features.Identity.Commands.ResendConfirmation;

public sealed class ResendConfirmationCommandHandler(
    IPublisher publisher,
    IIdentityService identityService,
    ILogger<ResendConfirmationCommandHandler> logger) : IRequestHandler<ResendConfirmationCommand, Result<Success>>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IPublisher _publisher = publisher;
    private readonly ILogger<ResendConfirmationCommandHandler> _logger = logger;

    public async Task<Result<Success>> Handle(ResendConfirmationCommand request, CancellationToken ct)
    {
        var userResult = await _identityService.GetUserByEmailAsync(request.Email, ct);

        if (userResult.IsError)
        {
            _logger.LogWarning(
                "Resend confirmation requested for non-existent email {Email}.",
                UtilityService.MaskEmail(request.Email));

            // Return success to prevent email enumeration
            return Result.Success;
        }

        var user = userResult.Value;
        var isConfirmedResult = await _identityService.IsEmailConfirmedAsync(user.Id);

        if (isConfirmedResult.IsSuccess && isConfirmedResult.Value)
        {
            _logger.LogInformation(
                "User {UserId} requested resend confirmation but is already confirmed.",
                user.Id);

            return Result.Success;
        }


        await _publisher.Publish(
         new GuestConfirmationEmailEvent(
             user.Id,
             user.Email,
             $"{user.FirstName} {user.LastName}"),
         ct);


        return Result.Success;
    }
}
