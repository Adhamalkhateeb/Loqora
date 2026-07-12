using Loqora.Application.Common.Errors;
using Loqora.Application.Common.Interfaces;
using Loqora.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Loqora.Application.Features.Identity.Commands.Logout;

public sealed class LogoutCommandHandler(
    IIdentityService identityService,
    IUser currentUser,
    ILogger<LogoutCommandHandler> logger) : IRequestHandler<LogoutCommand, Result<Success>>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IUser _currentUser = currentUser;
    private readonly ILogger<LogoutCommandHandler> _logger = logger;

    public async Task<Result<Success>> Handle(LogoutCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_currentUser.Id) || !Guid.TryParse(_currentUser.Id, out var userId))
        {
            _logger.LogWarning("Logout attempted without a valid authenticated user.");
            return ApplicationErrors.UserNotAuthenticated;
        }

        if (request.RefreshToken is null)
        {
            _logger.LogWarning("Logout attempted without a refresh token for user {UserId}.", userId);
            return ApplicationErrors.RefreshTokenMissing;
        }

        var result = await _identityService.LogoutAsync(userId, request.RefreshToken, ct);

        if (result.IsError)
        {
            _logger.LogError("Logout failed for user {UserId}. {@Errors}", userId, result.Errors);
            return result.Errors;
        }

        _logger.LogInformation("User {UserId} logged out successfully. Refresh tokens revoked.", userId);

        return Result.Success;
    }
}
