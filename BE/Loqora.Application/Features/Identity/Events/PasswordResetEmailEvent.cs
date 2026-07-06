using MediatR;

namespace Loqora.Application.Features.Identity.Events;

public sealed record PasswordResetEmailEvent(
    Guid UserId,
    string Email,
    string FullName,
    string ResetToken) : INotification;
