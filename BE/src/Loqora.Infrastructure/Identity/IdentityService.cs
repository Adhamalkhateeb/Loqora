using Loqora.Application.Common.Interfaces;
using Loqora.Application.Features.Identity.Dtos;
using Loqora.Domain.Common.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Loqora.Infrastructure.Identity
{
    public class IdentityService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ITokenProvider tokenProvider,
        IAppDbContext context) : IIdentityService
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        private readonly IAppDbContext _context = context;


        #region Authentication
        public async Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return Error.Unauthorized("Identity:InvalidCredentials", "The provided email or password is incorrect.");

            var passwordResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

            if (!passwordResult.Succeeded)
                return Error.Unauthorized("Identity:InvalidCredentials", "The provided email or password is incorrect.");


            if (await _userManager.IsLockedOutAsync(user))
                return Error.Unauthorized("Identity:UserLockedOut", "The user account is locked out due to multiple failed login attempts.");

            if (!user.EmailConfirmed)
                return Error.Forbidden("Identity:EmailNotConfirmed", "The email address has not been confirmed.");


            return await GenerateAuthResponseAsync(user, ct);
        }

        public async Task<Result<Success>> LogoutAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return Error.NotFound("Identity:User:NotFound", "The user was not found.");

            await _context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .ExecuteDeleteAsync(ct);

            return Result.Success;
        }

        public async Task<Result<AppUserDto>> CreateUserAsync(
            string firstName,
            string lastName,
            string email,
            string password,
            IList<string> roles,
            CancellationToken ct = default)
        {
            var user = AppUser.Create(firstName, lastName, email);

            var createResult = await _userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
                return MapIdentityErrors(createResult);

            var roleResult = await _userManager.AddToRolesAsync(user, roles);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                return MapIdentityErrors(roleResult);
            }

            return new AppUserDto(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email!,
                await _userManager.GetRolesAsync(user),
                await _userManager.GetClaimsAsync(user));
        }

        #endregion

        #region Email Confirmation

        public async Task<Result<Success>> ConfirmEmailAsync(Guid userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return Error.NotFound("Identity:User:NotFound", "The user was not found.");

            if (user.EmailConfirmed)
                return Error.Conflict("Identity:EmailAlreadyConfirmed", "The email address has already been confirmed.");

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
                return Error.Validation("Identity:InvalidConfirmationToken", "The provided confirmation token is invalid or has expired.");

            return Result.Success;
        }

        public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return Error.NotFound("Identity:User:NotFound", "The user was not found.");

            if (user.EmailConfirmed)
                return Error.Conflict("Identity:EmailAlreadyConfirmed", "The email address has already been confirmed.");

            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<Result<bool>> IsEmailConfirmedAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return Error.NotFound("Identity:User:NotFound", "The user was not found.");

            return user.EmailConfirmed;
        }

        #endregion

        #region Password Reset
        public async Task<Result<(Guid UserId, string UserFullName, string Token)>> GeneratePasswordResetTokenAsync(
            string email,
            CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return Error.NotFound("Identity:User:NotFound", "The user was not found.");

            if (user.EmailConfirmed == false)
                return Error.Forbidden("Identity:EmailNotConfirmed", "The email address has not been confirmed.");

            if (await _userManager.IsLockedOutAsync(user))
                return Error.Unauthorized("Identity:UserLockedOut", "The user account is locked out due to multiple failed login attempts.");

            return (user.Id, $"{user.FirstName} {user.LastName}", await _userManager.GeneratePasswordResetTokenAsync(user));
        }

        public async Task<Result<Success>> ResetPasswordAsync(
            string email,
            string token,
            string newPassword,
            CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return Error.NotFound("Identity:User:NotFound", "The user was not found.");

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);

            if (!result.Succeeded)
                return Error.Unexpected("Identity:PasswordResetFailed", "Password reset failed due to an unexpected error.");


            await _context.RefreshTokens.Where(rt => rt.UserId == user.Id).ExecuteDeleteAsync(ct);

            return Result.Success;
        }

        #endregion

        #region User

        public async Task<Result<AppUserDto>> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return Error.NotFound("Identity:User:NotFound", "The user was not found.");

            return new AppUserDto(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email!,
                    await _userManager.GetRolesAsync(user),
                    await _userManager.GetClaimsAsync(user));
        }

        public async Task<Result<AppUserDto>> GetUserByEmailAsync(
            string email,
            CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return Error.NotFound("Identity:User:NotFound", "User not found");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            return new AppUserDto(user.Id, user.FirstName, user.LastName, user.Email!, roles, claims);
        }

        #endregion

        #region helpers
        private async Task<Result<AuthResponse>> GenerateAuthResponseAsync(AppUser user, CancellationToken ct)
        {
            var userDto = new AppUserDto(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email!,
                await _userManager.GetRolesAsync(user),
                await _userManager.GetClaimsAsync(user));

            var tokenResult = await _tokenProvider.GenerateJwtTokenAsync(userDto, null, ct);

            if (tokenResult.IsError)
                return tokenResult.Errors;

            return new AuthResponse(
                Token: tokenResult.Value,
                UserId: user.Id,
                FullName: $"{user.FirstName} {user.LastName}",
                Email: user.Email!,
                EmailConfirmed: user.EmailConfirmed,
                Roles: await _userManager.GetRolesAsync(user));
        }

        private static List<Error> MapIdentityErrors(IdentityResult result)
        {
            return [.. result.Errors
                .Select(error => error.Code switch
                {
                    nameof(IdentityErrorDescriber.DuplicateEmail) or
                    nameof(IdentityErrorDescriber.DuplicateUserName)
                        => Error.Conflict("Identity:EmailAlreadyExists", "The provided email is already in use."),

                    nameof(IdentityErrorDescriber.PasswordTooShort) or
                    nameof(IdentityErrorDescriber.PasswordRequiresDigit) or
                    nameof(IdentityErrorDescriber.PasswordRequiresUpper) or
                    nameof(IdentityErrorDescriber.PasswordRequiresLower) or
                    nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric)
                    => Error.Validation("Identity:InvalidPassword", "The provided password does not meet the complexity requirements."),

                    _ =>  Error.Unexpected("Identity:RegistrationFailed", "User registration failed due to an unexpected error.")
                })
                .Distinct()];
        }

        #endregion

    }
}
