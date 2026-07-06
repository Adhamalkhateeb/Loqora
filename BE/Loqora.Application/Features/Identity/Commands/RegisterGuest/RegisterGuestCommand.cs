using MediatR;
using Loqora.Domain.Common.Results;
namespace Loqora.Application.Features.Identity.Commands.RegisterGuest;

public sealed record RegisterGuestCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password
) : IRequest<Result<Created>>;
