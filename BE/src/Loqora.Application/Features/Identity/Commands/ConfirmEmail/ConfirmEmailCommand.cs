using MediatR;
using Loqora.Domain.Common.Results;

namespace Loqora.Application.Features.Identity.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(
    Guid UserId,
    string Token
) : IRequest<Result<Success>>;
