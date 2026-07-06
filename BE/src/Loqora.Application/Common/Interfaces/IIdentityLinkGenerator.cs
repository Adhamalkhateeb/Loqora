namespace Loqora.Application.Common.Interfaces;

public interface IIdentityLinkGenerator
{
    string GenerateEmailConfirmationLink(
        Guid userId,
        string token);

    string GenerateResetPasswordLink(
        Guid userId,
        string token);
}