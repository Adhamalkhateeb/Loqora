using System.ComponentModel.DataAnnotations;


namespace Loqora.Contracts.Request.Auth;

public sealed class ResendConfirmationRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [MaxLength(256, ErrorMessage = "Email is too long.")]
    public string Email { get; set; } = null!;
}
