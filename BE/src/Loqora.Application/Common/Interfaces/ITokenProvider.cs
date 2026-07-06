using System.Security.Claims;
using Loqora.Application.Features.Identity.Dtos;
using Loqora.Domain.Common.Results;

namespace Loqora.Application.Common.Interfaces;

public interface ITokenProvider
{
    Task<Result<TokenResponse>> GenerateJwtTokenAsync(
        AppUserDto user,
        CancellationToken ct = default
    );

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
