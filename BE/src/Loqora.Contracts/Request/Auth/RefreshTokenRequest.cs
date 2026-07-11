using System.ComponentModel.DataAnnotations;


namespace Loqora.Contracts.Request.Auth;

public sealed class RefreshTokenRequest
{
    [Required]
    public string ExpiredAccessToken { get; set; } = null!;
}