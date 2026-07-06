using System.ComponentModel.DataAnnotations;

namespace Loqora.Contracts.Request.Auth;

public sealed class ResetPasswordRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Token is required.")]
    public string Token { get; set; } = null!;

    [Required(ErrorMessage = "New Password is required.")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = null!;
}
