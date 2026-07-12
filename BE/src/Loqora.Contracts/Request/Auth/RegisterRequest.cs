using System.ComponentModel.DataAnnotations;

namespace Loqora.Contracts.Request.Auth;


public sealed class RegisterRequest
{
    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(50, ErrorMessage = "First name is too long.")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(50, ErrorMessage = "Last name is too long.")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [MaxLength(256, ErrorMessage = "Email is too long.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(
        30,
        MinimumLength = 8,
        ErrorMessage = "Password must be between 8 and 30 characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = null!;
}
