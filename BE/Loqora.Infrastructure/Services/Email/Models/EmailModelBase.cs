namespace Loqora.Infrastructure.Services.Email.Models;


public abstract class EmailModelBase
{
    public string RecipientName { get; set; } = string.Empty;

    public int Year { get; set; } = DateTime.UtcNow.Year;

    public string CompanyName { get; set; } = "Loqora";

    public string SupportEmail { get; set; } = "support@loqora.com";
}
