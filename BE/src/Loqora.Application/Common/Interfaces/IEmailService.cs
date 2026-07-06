using Loqora.Domain.Common.Results;

namespace Loqora.Application.Common.Interfaces;

public interface IEmailService
{
    Task<Result<Success>> SendEmailConfirmationAsync(
        string email,
        string name,
        string confirmationLink,
        CancellationToken cancellationToken = default);

    Task<Result<Success>> SendPasswordResetAsync(
        string email,
        string name,
        string resetLink,
        CancellationToken cancellationToken = default);
}