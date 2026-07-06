namespace Loqora.Application.Features.Identity.Dtos;

public sealed record AuthResponse(TokenResponse Token, Guid UserId, string FullName, string Email, bool EmailConfirmed, IList<string> Roles);