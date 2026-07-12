using Loqora.Application.Common.Errors;
using Loqora.Application.Common.Interfaces;
using Loqora.Application.Features.Identity.Dtos;
using Loqora.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Loqora.Application.Features.Identity.Commands.RefreshTokens;

internal class RefreshTokenCommandHandler(
    ILogger<RefreshTokenCommandHandler> logger,
    IAppDbContext context,
    IIdentityService identityService,
    ITokenProvider tokenProvider,
    TimeProvider timeProvider) : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    private readonly ILogger<RefreshTokenCommandHandler> _logger = logger;
    private readonly IAppDbContext _context = context;
    private readonly IIdentityService _identityService = identityService;
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var principal = _tokenProvider.GetPrincipalFromExpiredToken(request.ExpiredAccessToken);

        if (principal is null)
        {
            _logger.LogWarning("Invalid expired access token tried to refresh.");
            return ApplicationErrors.ExpiredAccessTokenInvalid;
        }

        if (!Guid.TryParse(principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out Guid userId))
        {
            _logger.LogWarning("User ID claim not found in expired access token.");
            return ApplicationErrors.UserIdClaimNotFound;
        }

        var getUserResult = await _identityService.GetUserByIdAsync(userId);

        if (getUserResult.IsError)
        {
            _logger.LogError("Failed to get user by ID: {UserId}. Error: {ErrorDescription}", userId, getUserResult.TopError.Description);
            return getUserResult.Errors;
        }

        var user = getUserResult.Value;
        var hashedToken = _tokenProvider.HashToken(request.RefreshToken);

        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == hashedToken && rt.UserId == userId, ct);

        var now = _timeProvider.GetUtcNow();

        if (refreshToken is null || !refreshToken.IsActive(now))
        {
            _logger.LogWarning("Invalid, revoked, or expired refresh token for user ID: {UserId}", userId);
            return ApplicationErrors.RefreshTokenInvalid;
        }

        var tokenResult = await _tokenProvider.GenerateJwtTokenAsync(user, refreshToken.Id, ct);
        if (tokenResult.IsError)
        {
            _logger.LogError(
                "Failed to generate new JWT token for user ID: {UserId}. Error: {Error}",
                userId,
                tokenResult.TopError.Description);

            return tokenResult.Errors;
        }

        return tokenResult.Value;
    }
}

