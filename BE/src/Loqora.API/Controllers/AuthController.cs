using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Loqora.Application.Features.Identity.Commands.ConfirmEmail;
using Loqora.Application.Features.Identity.Commands.ForgotPassword;
using Loqora.Application.Features.Identity.Commands.Login;
using Loqora.Application.Features.Identity.Commands.Logout;
using Loqora.Application.Features.Identity.Commands.RegisterGuest;
using Loqora.Application.Features.Identity.Commands.ResendConfirmation;
using Loqora.Application.Features.Identity.Commands.ResetPassword;
using Loqora.Application.Features.Identity.Dtos;
using Loqora.Contracts.Request.Auth;

namespace Loqora.Web.Controllers;

[Route("api/v{version:apiversion}/auth")]
[ApiVersion("1.0")]
public sealed class AuthController(ISender sender) : ApiController
{
    [HttpPost("login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [EndpointName("Login")]
    [EndpointSummary("Authenticates a user and returns an access token.")]
    [EndpointDescription("This endpoint allows a user to authenticate by providing their email and password. Upon successful authentication, it returns an AuthResponse containing the access token, refresh token, and token expiration.")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await sender.Send(command, ct);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("RegisterGuest")]
    [EndpointSummary("Registers a new guest user.")]
    [EndpointDescription("This endpoint allows a new guest user to register by providing their first name, last name, email, and password. Upon successful registration, a confirmation email will be sent to the provided email address.")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken ct
    )
    {
        var command = new RegisterGuestCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password
        );

        var result = await sender.Send(command, ct);

        return result.Match(response => Created(), Problem);
    }

    [HttpPost("logout")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [EndpointName("Logout")]
    [EndpointSummary("Logs out the current user and revokes their refresh token.")]
    [EndpointDescription("This endpoint logs out the authenticated user by revoking all their active refresh tokens. Requires a valid JWT access token.")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var command = new LogoutCommand();
        var result = await sender.Send(command, ct);
        return result.Match(response => Ok(), Problem);
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [EndpointName("ForgotPassword")]
    [EndpointSummary("Initiates the password reset process.")]
    [EndpointDescription("This endpoint sends a password reset link to the provided email if it exists.")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken ct
    )
    {
        var command = new ForgotPasswordCommand(request.Email);
        var result = await sender.Send(command, ct);
        return result.Match(response => Ok(), Problem);
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [EndpointName("ResetPassword")]
    [EndpointSummary("Resets a user's password.")]
    [EndpointDescription("This endpoint allows a user to reset their password using a valid reset token.")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken ct
    )
    {
        var command = new ResetPasswordCommand(request.Email, request.Token, request.NewPassword);
        var result = await sender.Send(command, ct);
        return result.Match(response => Ok(), Problem);
    }

    [HttpGet("confirm-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("ConfirmEmail")]
    [EndpointSummary("Confirms the email address of a guest user.")]
    [EndpointDescription("This endpoint allows a guest user to confirm their email address by providing their user ID and a confirmation token. Upon successful confirmation, the user's email address will be marked as confirmed.")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> ConfirmEmail(
        [FromQuery] Guid userId,
        [FromQuery] string token,
        CancellationToken ct
    )
    {
        var command = new ConfirmEmailCommand(userId, token);
        var result = await sender.Send(command, ct);
        return result.Match(response => Ok(), Problem);
    }

    [HttpPost("resend-confirmation")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("ResendConfirmation")]
    [EndpointSummary("Resends the confirmation email to a guest user.")]
    [EndpointDescription("This endpoint resends the email confirmation link if the user has not confirmed their email yet.")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> ResendConfirmation(
        [FromBody] ResendConfirmationRequest request,
        CancellationToken ct
    )
    {
        var command = new ResendConfirmationCommand(request.Email);
        var result = await sender.Send(command, ct);
        return result.Match(response => Ok(), Problem);
    }
}
