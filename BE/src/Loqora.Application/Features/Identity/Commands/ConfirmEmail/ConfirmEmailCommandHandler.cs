using Loqora.Application.Common.Interfaces;
using Loqora.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Loqora.Application.Features.Identity.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler(
    IIdentityService identityService,
    ILogger<ConfirmEmailCommandHandler> logger) : IRequestHandler<ConfirmEmailCommand, Result<Success>>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly ILogger<ConfirmEmailCommandHandler> _logger = logger;

    public async Task<Result<Success>> Handle(ConfirmEmailCommand request, CancellationToken ct)
    {
        var result = await _identityService.ConfirmEmailAsync(request.UserId, request.Token);

        if (result.IsError)
        {
            _logger.LogWarning(
                "Failed to confirm email for user {UserId}. {@Errors}",
                request.UserId,
                result.Errors);
            return result.Errors;
        }

        _logger.LogInformation(
            "Successfully confirmed email for user {UserId}.",
            request.UserId);

        return Result.Success;
    }
}
