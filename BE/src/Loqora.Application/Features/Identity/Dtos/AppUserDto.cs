
using System.Security.Claims;

namespace Loqora.Application.Features.Identity.Dtos;

public sealed record AppUserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    IList<string> Roles,
    IList<Claim> Claims
);

