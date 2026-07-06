using MediatR;

namespace Loqora.Application.Features.Identity.Events;

public sealed record GuestConfirmationEmailEvent(
    Guid UserId,
    string Email,
    string FullName) : INotification;
