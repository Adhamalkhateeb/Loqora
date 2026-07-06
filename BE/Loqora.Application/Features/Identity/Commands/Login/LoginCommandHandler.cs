using MediatR;
using Microsoft.Extensions.Logging;
using Loqora.Application.Common.Interfaces;
using Loqora.Application.Features.Identity.Dtos;
using Loqora.Domain.Common.Results;

namespace Loqora.Application.Features.Identity.Commands.Login;

public sealed class LoginCommandHandler(
    IIdentityService identityService,
    ILogger<LoginCommandHandler> logger) : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly ILogger<LoginCommandHandler> _logger = logger;

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        var result = await _identityService.LoginAsync(request.Email, request.Password, ct);

        if (result.IsError)
        {
            _logger.LogWarning(
                "Failed login attempt for user {Email}. {@Errors}",
                UtilityService.MaskEmail(request.Email),
                result.Errors);
            return result.Errors;
        }

        _logger.LogInformation(
            "User {Email} logged in successfully.",
            UtilityService.MaskEmail(request.Email));

        return result.Value;
    }
}
