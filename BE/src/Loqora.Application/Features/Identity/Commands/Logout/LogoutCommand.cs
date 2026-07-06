using MediatR;
using Loqora.Domain.Common.Results;

namespace Loqora.Application.Features.Identity.Commands.Logout;

public sealed record LogoutCommand() : IRequest<Result<Success>>;
