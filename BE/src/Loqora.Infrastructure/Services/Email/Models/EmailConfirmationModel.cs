namespace Loqora.Infrastructure.Services.Email.Models;

public class EmailConfirmationModel : EmailModelBase
{
    public string ConfirmationLink { get; set; } = string.Empty;
}
