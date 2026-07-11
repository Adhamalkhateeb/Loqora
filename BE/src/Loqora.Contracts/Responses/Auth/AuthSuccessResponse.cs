namespace Loqora.Contracts.Responses.Auth;

public sealed record AccessTokenResponse(string AccessToken, DateTime ExpiresOnUtc);

public sealed record AuthSuccessResponse(
    AccessTokenResponse Token,
    Guid UserId,
    string FullName,
    string Email,
    bool EmailConfirmed,
    IList<string> Roles
);
