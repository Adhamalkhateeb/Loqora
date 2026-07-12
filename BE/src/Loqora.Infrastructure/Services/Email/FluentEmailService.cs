using FluentEmail.Core;
using Loqora.Application.Common.Interfaces;
using Loqora.Domain.Common.Results;
using Loqora.Infrastructure.Services.Email.Models;

namespace Loqora.Infrastructure.Services.Email;

public class FluentEmailService(IFluentEmail fluentEmail) : IEmailService
{
    private static readonly string TemplatesDir =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Services", "Email", "Templates");

    public Task<Result<Success>> SendEmailConfirmationAsync(
        string email, string name, string confirmationLink, CancellationToken cancellationToken = default)
    {
        var model = new EmailConfirmationModel
        {
            RecipientName = name,
            ConfirmationLink = confirmationLink
        };

        return SendAsync(email, name, "Email Confirmation", "EmailConfirmation.cshtml", model, cancellationToken);
    }

    public Task<Result<Success>> SendPasswordResetAsync(
        string email, string name, string resetLink, CancellationToken cancellationToken = default)
    {
        var model = new PasswordResetModel
        {
            RecipientName = name,
            ResetLink = resetLink
        };

        return SendAsync(email, name, "Reset Your Password", "PasswordReset.cshtml", model, cancellationToken);
    }


    private async Task<Result<Success>> SendAsync<TModel>(
        string toEmail,
        string toName,
        string subject,
        string templateFileName,
        TModel model,
        CancellationToken cancellationToken) where TModel : EmailModelBase
    {
        var templatePath = Path.Combine(TemplatesDir, templateFileName);

        var response = await fluentEmail
            .To(toEmail, toName)
            .Subject(subject)
            .UsingTemplateFromFile(templatePath, model)
            .SendAsync(cancellationToken);

        if (!response.Successful)
        {
            return Error.Unexpected("Email.SendFailed", string.Join(", ", response.ErrorMessages));
        }

        return Result.Success;
    }
}
