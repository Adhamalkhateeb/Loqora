using Loqora.Application.Features.Identity.Dtos;
using Loqora.Domain.Common.Results;

namespace Loqora.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<AppUserDto>> CreateUserAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        IList<string> roles,
        CancellationToken cancellationToken = default);



    Task<Result<string>> GenerateEmailConfirmationTokenAsync(
        Guid userId);

    Task<Result<Success>> ConfirmEmailAsync(
        Guid userId,
        string token);

    Task<Result<bool>> IsEmailConfirmedAsync(
        Guid userId);

    Task<Result<AuthResponse>> LoginAsync(
    string email,
    string password,
    CancellationToken ct = default);

    Task<Result<AppUserDto>> GetUserAsync(
        Guid userId);

    //Task<Result<AuthResponse>> RefreshTokenAsync(
    //    string token,
    //    string refreshToken,
    //    CancellationToken ct = default
    //);

    //Task<Result<Updated>> RevokeTokenAsync(
    //    string refreshToken,
    //    CancellationToken ct = default
    //);


    Task<Result<(Guid UserId, string UserFullName, string Token)>> GeneratePasswordResetTokenAsync(
        string email,
        CancellationToken ct = default
    );

    Task<Result<Success>> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken ct = default
    );

    //Task<Result<Updated>> ChangePasswordAsync(
    //    string userId,
    //    string currentPassword,
    //    string newPassword,
    //    CancellationToken ct = default
    //);

    //// Management / Roles
    //Task<Result<IEnumerable<AppUserDto>>> GetUsersAsync(CancellationToken ct = default);
    //Task<Result<AppUserDto>> GetUserByIdAsync(string id, CancellationToken ct = default);
    Task<Result<AppUserDto>> GetUserByEmailAsync(string email, CancellationToken ct = default);
    //Task<Result<Updated>> UpdateAsync(AppUserDto user, CancellationToken ct = default);
    //Task<Result<Deleted>> DeleteUserAsync(string id, CancellationToken ct = default);
    //Task<Result<Updated>> AssignRoleAsync(string userId, string role, CancellationToken ct = default);
}
