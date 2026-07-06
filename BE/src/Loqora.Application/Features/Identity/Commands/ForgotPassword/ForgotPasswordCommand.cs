using MediatR;
using Loqora.Domain.Common.Results;

namespace Loqora.Application.Features.Identity.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<Result<Success>>;
