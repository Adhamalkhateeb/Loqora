namespace Loqora.Application.Features.Identity.Dtos;

public class TokenResponse
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset ExpiresOnUtc { get; set; }
}
