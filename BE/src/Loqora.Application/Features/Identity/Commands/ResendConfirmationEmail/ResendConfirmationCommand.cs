using MediatR;
using Loqora.Domain.Common.Results;

namespace Loqora.Application.Features.Identity.Commands.ResendConfirmation;

public sealed record ResendConfirmationCommand(
    string Email
) : IRequest<Result<Success>>;
