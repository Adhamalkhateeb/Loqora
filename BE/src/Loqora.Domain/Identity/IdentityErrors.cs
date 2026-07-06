using Loqora.Domain.Common.Results;

namespace Loqora.Domain.Identity;

public static class IdentityErrors
{


    public static readonly Error InvalidEmailAddress =
        Error.Validation("Identity:InvalidEmailAddress", "Invalid email address.");

    public static readonly Error EmailAlreadyExists = Error.Conflict(
        "Identity:EmailAlreadyExists", "The provided email is already in use.");

    public static readonly Error RegistrationFailed = Error.Unexpected(
        "Identity:RegistrationFailed", "User registration failed due to an unexpected error.");

    public static readonly Error InvalidPassword = Error.Validation(
        "Identity:InvalidPassword", "The provided password does not meet the complexity requirements.");

    public static readonly Error InvalidCredentials = Error.Unauthorized(
        "Identity:InvalidCredentials", "The provided email or password is incorrect.");

    public static readonly Error UserNotFound = Error.NotFound(
        "Identity:UserNotFound", "The user was not found.");

    public static readonly Error InvalidConfirmationToken = Error.Validation(
        "Identity:InvalidConfirmationToken", "The provided confirmation token is invalid or has expired.");

    public static readonly Error EmailNotConfirmed = Error.Forbidden(
        "Identity:EmailNotConfirmed", "The email address has not been confirmed.");

    public static readonly Error EmailAlreadyConfirmed = Error.Conflict(
        "Identity:EmailAlreadyConfirmed", "The email address has already been confirmed.");

    public static readonly Error UserLockedOut = Error.Unauthorized(
        "Identity:UserLockedOut", "The user account is locked out due to multiple failed login attempts.");

    public static readonly Error PasswordResetFailed = Error.Unexpected(
        "Identity:PasswordResetFailed", "Password reset failed due to an unexpected error.");
}
