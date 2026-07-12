using Loqora.Domain.Common;
using Loqora.Domain.Common.Results;

namespace Loqora.Domain.Identity;

public sealed class RefreshToken : AuditableEntity
{
    public string Token { get; }
    public Guid UserId { get; }
    public DateTimeOffset ExpiresOnUtc { get; }
    public DateTimeOffset? RevokedOnUtc { get; private set; }
    public bool IsRevoked => RevokedOnUtc != null;
    public bool IsExpired(DateTimeOffset now) => now >= ExpiresOnUtc;
    public bool IsActive(DateTimeOffset now) => !IsDeleted && !IsRevoked && !IsExpired(now);

    private RefreshToken() { }

    private RefreshToken(Guid id, string token, Guid userId, DateTimeOffset expiresOnUtc)
        : base(id)
    {
        Token = token;
        UserId = userId;
        ExpiresOnUtc = expiresOnUtc;
    }

    public static Result<RefreshToken> Create(
        Guid id,
        string token,
        Guid userId,
        DateTimeOffset expiresOnUtc,
        DateTimeOffset now)
    {
        if (id == Guid.Empty)
        {
            return RefreshTokenErrors.IdRequired;
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return RefreshTokenErrors.TokenRequired;
        }

        if (userId == Guid.Empty)
        {
            return RefreshTokenErrors.UserIdRequired;
        }

        if (expiresOnUtc <= now)
        {
            return RefreshTokenErrors.ExpiryInvalid;
        }

        return new RefreshToken(id, token, userId, expiresOnUtc);
    }

    public Result<Success> Revoke(DateTimeOffset revokedOnUtc)
    {
        if (revokedOnUtc < CreatedAtUtc)
        {
            return RefreshTokenErrors.RevokedOnInvalid;
        }

        if (IsRevoked)
        {
            return RefreshTokenErrors.AlreadyRevoked;
        }

        RevokedOnUtc = revokedOnUtc;
        return Result.Success;
    }
}


