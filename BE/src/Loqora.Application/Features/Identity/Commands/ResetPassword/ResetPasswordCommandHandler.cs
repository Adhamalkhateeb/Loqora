using MediatR;
using Microsoft.Extensions.Logging;
using Loqora.Application.Common.Interfaces;
using Loqora.Domain.Common.Results;

namespace Loqora.Application.Features.Identity.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler(
    IIdentityService identityService,
    ILogger<ResetPasswordCommandHandler> logger) : IRequestHandler<ResetPasswordCommand, Result<Success>>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly ILogger<ResetPasswordCommandHandler> _logger = logger;

    public async Task<Result<Success>> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        var result = await _identityService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword, ct);

        if (result.IsError)
        {
            _logger.LogWarning("Failed to reset password for {Email} {@Errors}.", UtilityService.MaskEmail(request.Email), result.Errors);
            return result.Errors;
        }

        return Result.Success;
    }
}
