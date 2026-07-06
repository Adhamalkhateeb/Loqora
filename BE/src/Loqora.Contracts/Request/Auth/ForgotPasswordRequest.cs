using System.ComponentModel.DataAnnotations;

namespace Loqora.Contracts.Request.Auth;

public sealed class ForgotPasswordRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = null!;
}
