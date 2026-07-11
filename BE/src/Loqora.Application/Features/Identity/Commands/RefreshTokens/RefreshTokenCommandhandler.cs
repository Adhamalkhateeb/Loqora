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
    ITokenProvider tokenProvider) : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    private readonly ILogger<RefreshTokenCommandHandler> _logger = logger;
    private readonly IAppDbContext _context = context;
    private readonly IIdentityService _identityService = identityService;
    private readonly ITokenProvider _tokenProvider = tokenProvider;

    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var principle = _tokenProvider.GetPrincipalFromExpiredToken(request.ExpiredAccessToken);

        if (principle is null)
        {
            _logger.LogError("Invalid expired access token.");
            return ApplicationErrors.ExpiredAccessTokenInvalid;
        }


        if (!Guid.TryParse(principle.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out Guid userId))
        {
            _logger.LogError("User ID claim not found in expired access token.");
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


        var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(
            rt => rt.Token == hashedToken && rt.UserId == userId,
            ct);

        if (refreshToken is null || refreshToken.ExpiresOnUtc < DateTimeOffset.UtcNow)
        {
            _logger.LogError("Invalid or expired refresh token for user ID: {UserId}", userId);
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

