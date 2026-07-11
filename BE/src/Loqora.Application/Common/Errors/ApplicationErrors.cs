

using Loqora.Domain.Common.Results;

namespace Loqora.Application.Common.Errors;

public sealed class ApplicationErrors
{
    public static Error ExpiredAccessTokenInvalid =>
       Error.Unauthorized("Auth:ExpiredAccessToken:Invalid", "Invalid expired access token.");

    public static Error UserIdClaimNotFound =>
        Error.Unauthorized(
            "Auth:UserIdClaim:NotFound",
            "User ID claim not found in expired access token.");

    public static Error RefreshTokenInvalid =>
        Error.Unauthorized("Auth:RefreshToken:Invalid", "Invalid or expired refresh token.");

    public static Error UserNotAuthenticated =>
        Error.Unauthorized("Auth:User:NotAuthenticated", "User is not authenticated.");
}

