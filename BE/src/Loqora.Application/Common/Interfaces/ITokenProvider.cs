using Loqora.Application.Features.Identity.Dtos;
using Loqora.Domain.Common.Results;
using System.Security.Claims;

namespace Loqora.Application.Common.Interfaces;

public interface ITokenProvider
{
    Task<Result<TokenResponse>> GenerateJwtTokenAsync(
        AppUserDto user,
        Guid? replacedRefreshTokenId = null,
        CancellationToken ct = default);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    string HashToken(string token);
}
