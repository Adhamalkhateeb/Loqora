namespace Loqora.Infrastructure.Services.Email.Models;

public class PasswordResetModel : EmailModelBase
{
    public string ResetLink { get; set; } = string.Empty;
}
