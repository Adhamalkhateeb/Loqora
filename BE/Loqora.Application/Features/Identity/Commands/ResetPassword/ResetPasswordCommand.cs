using MediatR;
using Loqora.Domain.Common.Results;

namespace Loqora.Application.Features.Identity.Commands.ResetPassword;

public sealed record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<Result<Success>>;
