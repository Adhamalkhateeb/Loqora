using Loqora.Domain.Common.Results;
using MediatR;

namespace Loqora.Application.Features.Identity.Commands.Logout;

public sealed record LogoutCommand : IRequest<Result<Success>>;
