namespace Loqora.Contracts.Responses.Auth;

public sealed record AccessTokenResponse(string AccessToken, DateTimeOffset ExpiresOnUtc);

