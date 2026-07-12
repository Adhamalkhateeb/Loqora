using Loqora.Domain.Common.Results;

namespace Loqora.Domain.Identity;

public static class RefreshTokenErrors
{
    public static readonly Error IdRequired = Error.Validation(
        "RefreshToken:Id:Required",
        "Refresh token ID is required.");

    public static readonly Error TokenRequired = Error.Validation(
        "RefreshToken:Token:Required",
        "Token value is required.");

    public static readonly Error UserIdRequired = Error.Validation(
        "RefreshToken:UserId:Required",
        "User ID is required.");

    public static readonly Error ExpiryInvalid = Error.Validation(
        "RefreshToken:Expiry:Invalid",
        "Expiry must be in the future.");

    public static readonly Error RevokedOnInvalid = Error.Validation(
        "RefreshToken:RevokeDate:Invalid",
        "Revoke date must not be before the creation date.");

    public static readonly Error AlreadyRevoked = Error.Validation(
        "RefreshToken:AlreadyRevoked",
        "Refresh token is already revoked.");
}
