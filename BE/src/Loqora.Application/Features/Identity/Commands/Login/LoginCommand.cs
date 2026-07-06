using MediatR;
using Loqora.Application.Features.Identity.Dtos;
using Loqora.Domain.Common.Results;

namespace Loqora.Application.Features.Identity.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<AuthResponse>>;
