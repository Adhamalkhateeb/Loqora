using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Loqora.Application.Common.Interfaces;
using Loqora.Application.Features.Identity.Dtos;
using Loqora.Domain.Common.Results;
using Loqora.Domain.Identity;
using Loqora.Infrastructure.Settings;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Loqora.Infrastructure.Identity;

public class TokenProvider(IAppDbContext context, IOptions<JwtSettings> options, TimeProvider timeProvider) : ITokenProvider
{
    private readonly IAppDbContext _context = context;
    private readonly JwtSettings _jwtSettings = options.Value;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Result<TokenResponse>> GenerateJwtTokenAsync(AppUserDto user, Guid? replacedRefreshTokenId = null, CancellationToken ct = default)
    {
        return await IssueTokenPairAsync(user, replacedRefreshTokenId, ct);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret!)),

            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,

            ValidateLifetime = false, // Ignore token expiration
            ClockSkew = TimeSpan.Zero,
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwt ||
                !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch (SecurityTokenException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    public string HashToken(string token)
    {
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashedBytes);
    }

    private async Task<Result<TokenResponse>> IssueTokenPairAsync(AppUserDto user, Guid? replacedRefreshTokenId, CancellationToken ct)
    {
        var now = _timeProvider.GetUtcNow();

        var accessTokenExpires =
            now.AddMinutes(_jwtSettings.TokenExpirationInMinutes);

        var refreshTokenExpires =
            now.AddDays(_jwtSettings.RefreshTokenExpirationInDays);

        if (replacedRefreshTokenId.HasValue)
        {
            var oldRefreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(
                    rt => rt.Id == replacedRefreshTokenId &&
                          rt.UserId == user.Id,
                    ct);

            if (oldRefreshToken is null)
            {
                return Error.Validation(
                    "Identity.RefreshToken.Invalid",
                    "The refresh token is invalid.");
            }

            var revokeResult = oldRefreshToken.Revoke(now);

            if (revokeResult.IsError)
            {
                return revokeResult.Errors;
            }
        }

        var refreshToken = GenerateRefreshToken();

        var refreshTokenResult = RefreshToken.Create(
            Guid.NewGuid(),
            HashToken(refreshToken),
            user.Id,
            refreshTokenExpires,
            now);

        if (refreshTokenResult.IsError)
        {
            return refreshTokenResult.Errors;
        }

        _context.RefreshTokens.Add(refreshTokenResult.Value);
        await _context.SaveChangesAsync(ct);

        return new TokenResponse
        {
            AccessToken = CreateAccessToken(
                user,
                accessTokenExpires,
                now),

            RefreshToken = refreshToken,

            ExpiresOnUtc = accessTokenExpires
        };
    }

    private string CreateAccessToken(AppUserDto user, DateTimeOffset expires, DateTimeOffset now)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(
                JwtRegisteredClaimNames.Iat,
                now.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires.UtcDateTime,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var handler = new JwtSecurityTokenHandler();

        return handler.WriteToken(handler.CreateToken(descriptor));
    }

    private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

}
