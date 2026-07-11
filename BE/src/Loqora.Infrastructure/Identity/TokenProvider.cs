using Loqora.Application.Common.Interfaces;
using Loqora.Application.Features.Identity.Dtos;
using Loqora.Domain.Common.Results;
using Loqora.Domain.Identity;
using Loqora.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Loqora.Infrastructure.Identity;

public class TokenProvider(IAppDbContext context, IOptions<JwtSettings> options) : ITokenProvider
{
    private readonly IAppDbContext _context = context;
    private readonly JwtSettings _jwtSettings = options.Value;

    public async Task<Result<TokenResponse>> GenerateJwtTokenAsync(AppUserDto user, Guid? replacedRefreshTokenId = null, CancellationToken ct = default)
    {
        var tokenResult = await CreateAsync(user, replacedRefreshTokenId, ct);

        if (tokenResult.IsError)
        {
            return tokenResult.Errors;
        }

        return tokenResult.Value;
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
            // malformed / non-JWT string passed in
            return null;
        }
    }

    public string HashToken(string token)
    {
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashedBytes);
    }

    private async Task<Result<TokenResponse>> CreateAsync(AppUserDto user, Guid? replacedRefreshTokenId = null, CancellationToken ct = default)
    {
        var accessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes);
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new(ClaimTypes.Role, role));
        }

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = accessTokenExpiry,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                SecurityAlgorithms.HmacSha256Signature),
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var securityToken = tokenHandler.CreateToken(descriptor);

        if (replacedRefreshTokenId is Guid oldTokenId)
        {
            await _context.RefreshTokens
                .Where(rt => rt.Id == oldTokenId && rt.UserId == user.Id)
                .ExecuteDeleteAsync(ct);
        }



        var refreshToken = GenerateRefreshToken();
        var hashedRefreshToken = HashToken(refreshToken);

        var createRefreshTokenResult = RefreshToken.Create(
            Guid.NewGuid(),
            hashedRefreshToken,
            user.Id,
            refreshTokenExpiry);

        if (createRefreshTokenResult.IsError)
        {
            return createRefreshTokenResult.Errors;
        }


        _context.RefreshTokens.Add(createRefreshTokenResult.Value);
        await _context.SaveChangesAsync(ct);

        return new TokenResponse
        {
            AccessToken = tokenHandler.WriteToken(securityToken),
            RefreshToken = refreshToken,
            ExpiresOnUtc = accessTokenExpiry,
        };
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
}
