using MediatR;
using Loqora.Domain.Common.Results;

namespace Loqora.Application.Features.Identity.Commands.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<Success>>
{
    public Task<Result<Success>> Handle(LogoutCommand request, CancellationToken ct)
    {
        // For JWT based auth without refresh tokens or token blacklisting, logout is primarily a client-side action.
        // Returning Success to indicate the action was processed.
        return Task.FromResult<Result<Success>>(Result.Success);
    }
}
