using MediatR;
using Microsoft.Extensions.Logging;
using Loqora.Application.Common.Interfaces;
using Loqora.Application.Features.Identity.Events;
using Loqora.Domain.Common.Results;
using Loqora.Domain.Identity;

namespace Loqora.Application.Features.Identity.Commands.RegisterGuest;

public sealed class RegisterGuestCommandHandler(
    IIdentityService identityService,
    IPublisher publisher,
    ILogger<RegisterGuestCommandHandler> logger) : IRequestHandler<RegisterGuestCommand, Result<Created>>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IPublisher _publisher = publisher;
    private readonly ILogger<RegisterGuestCommandHandler> _logger = logger;

    public async Task<Result<Created>> Handle(RegisterGuestCommand request, CancellationToken ct)
    {
        var createUserResult = await _identityService.CreateUserAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            [Role.Guest.ToString()],
            ct);

        if (createUserResult.IsError)
        {
            _logger.LogWarning(
                "Failed to register guest with email {Email}. {@Errors}",
                UtilityService.MaskEmail(request.Email),
                createUserResult.Errors);

            return createUserResult.Errors;
        }

        var user = createUserResult.Value;

        _logger.LogInformation(
            "Guest user {UserId} ({Email}) registered successfully.",
            user.Id,
            UtilityService.MaskEmail(user.Email));

        await _publisher.Publish(
            new GuestConfirmationEmailEvent(
                user.Id,
                user.Email,
                $"{user.FirstName} {user.LastName}"),
            ct);

        return Result.Created;
    }
}
