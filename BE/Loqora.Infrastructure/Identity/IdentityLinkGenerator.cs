using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Loqora.Application.Common.Interfaces;
using Loqora.Infrastructure.Settings;
using System.Text;

namespace Loqora.Infrastructure.Identity;

public sealed class IdentityLinkGenerator(
    IOptions<EmailOptions> options)
    : IIdentityLinkGenerator
{
    private readonly EmailOptions _options = options.Value;

    public string GenerateEmailConfirmationLink(
        Guid userId,
        string token)
    {
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        return
            $"{_options.FrontendBaseUrl}/confirm-email" +
            $"?userId={Uri.EscapeDataString(userId.ToString())}" +
            $"&token={Uri.EscapeDataString(encodedToken)}";
    }

    public string GenerateResetPasswordLink(
        Guid userId,
        string token)
    {
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        return
            $"{_options.FrontendBaseUrl}/reset-password" +
            $"?userId={Uri.EscapeDataString(userId.ToString())}" +
            $"&token={Uri.EscapeDataString(encodedToken)}";
    }
}