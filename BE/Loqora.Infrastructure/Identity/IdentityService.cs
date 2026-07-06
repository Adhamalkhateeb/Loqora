using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Loqora.Application.Common.Interfaces;
using Loqora.Application.Features.Identity.Dtos;
using Loqora.Domain.Common.Results;
using Loqora.Domain.Identity;
using System.Text;

namespace Loqora.Infrastructure.Identity
{
    public class IdentityService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ITokenProvider tokenProvider) : IIdentityService
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly ITokenProvider _tokenProvider = tokenProvider;


        public async Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return IdentityErrors.InvalidCredentials;

            if (await _userManager.IsLockedOutAsync(user))
                return IdentityErrors.UserLockedOut;

            if (!user.EmailConfirmed)
                return IdentityErrors.EmailNotConfirmed;

            var passwordResult = await _signInManager.CheckPasswordSignInAsync(
                user,
                password,
                lockoutOnFailure: true);

            if (!passwordResult.Succeeded)
                return IdentityErrors.InvalidCredentials;

            return await GenerateAuthResponseAsync(user, ct);

        }

        public async Task<Result<AppUserDto>> CreateUserAsync(
            string firstName,
            string lastName,
            string email,
            string password,
            IList<string> roles,
            CancellationToken ct = default
        )
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

        public async Task<Result<Success>> ConfirmEmailAsync(Guid userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return IdentityErrors.UserNotFound;

            if (user.EmailConfirmed)
                return IdentityErrors.EmailAlreadyConfirmed;

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
                return IdentityErrors.InvalidConfirmationToken;

            return Result.Success;
        }



        public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return IdentityErrors.UserNotFound;

            if (user.EmailConfirmed)
                return IdentityErrors.EmailAlreadyConfirmed;

            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<Result<bool>> IsEmailConfirmedAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return IdentityErrors.UserNotFound;

            return user.EmailConfirmed;
        }

        public async Task<Result<(Guid UserId, string UserFullName, string Token)>> GeneratePasswordResetTokenAsync(
            string email,
            CancellationToken ct = default
        )
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return IdentityErrors.UserNotFound;

            if (user.EmailConfirmed == false)
                return IdentityErrors.EmailNotConfirmed;

            if (await _userManager.IsLockedOutAsync(user))
                return IdentityErrors.UserLockedOut;

            return (user.Id, $"{user.FirstName} {user.LastName}", await _userManager.GeneratePasswordResetTokenAsync(user));

        }

        public async Task<Result<Success>> ResetPasswordAsync(
            string email,
            string token,
            string newPassword,
            CancellationToken ct = default
        )
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return IdentityErrors.UserNotFound;

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);

            if (!result.Succeeded)
                return IdentityErrors.PasswordResetFailed;

            return Result.Success;
        }


        public async Task<Result<AppUserDto>> GetUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return IdentityErrors.UserNotFound;

            return new AppUserDto(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email!,
                    await _userManager.GetRolesAsync(user),
                    await _userManager.GetClaimsAsync(user));
        }





        private async Task<Result<AuthResponse>> GenerateAuthResponseAsync(AppUser user, CancellationToken ct)
        {
            var userDto = new AppUserDto(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email!,
                await _userManager.GetRolesAsync(user),
                await _userManager.GetClaimsAsync(user)
            );

            var tokenResult = await _tokenProvider.GenerateJwtTokenAsync(userDto, ct);

            if (tokenResult.IsError)
                return tokenResult.Errors;

            return new AuthResponse(
                Token: tokenResult.Value,
                UserId: user.Id,
                FullName: $"{user.FirstName} {user.LastName}",
                Email: user.Email!,
                EmailConfirmed: user.EmailConfirmed,
                Roles: await _userManager.GetRolesAsync(user)
                );
        }


        private static List<Error> MapIdentityErrors(IdentityResult result)
        {
            return result.Errors
                .Select(error => error.Code switch
                {
                    nameof(IdentityErrorDescriber.DuplicateEmail)
                        => IdentityErrors.EmailAlreadyExists,

                    nameof(IdentityErrorDescriber.DuplicateUserName)
                        => IdentityErrors.EmailAlreadyExists,

                    nameof(IdentityErrorDescriber.PasswordTooShort) or
                    nameof(IdentityErrorDescriber.PasswordRequiresDigit) or
                    nameof(IdentityErrorDescriber.PasswordRequiresUpper) or
                    nameof(IdentityErrorDescriber.PasswordRequiresLower) or
                    nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric)
                    => IdentityErrors.InvalidPassword,

                    _ => IdentityErrors.RegistrationFailed
                })
                .Distinct()
                .ToList();
        }









        //public async Task<Result<AuthResponse>> RefreshTokenAsync(
        //    string token,
        //    string refreshToken,
        //    CancellationToken ct = default
        //)
        //{
        //    // Implementation placeholder, usually interacts with ITokenProvider and IAppDbContext
        //    return Error.Unexpected("Identity.RefreshToken", "Not fully implemented.");
        //}

        //public async Task<Result<Updated>> RevokeTokenAsync(
        //    string refreshToken,
        //    CancellationToken ct = default
        //)
        //{
        //    // Implementation placeholder
        //    return Result.Updated;
        //}

        //public async Task<Result<Updated>> ConfirmEmailAsync(
        //    string email,
        //    string token,
        //    CancellationToken ct = default
        //)
        //{
        //    var user = await _userManager.FindByEmailAsync(email);

        //    if (user is null)
        //    {
        //        return Error.NotFound("User:Not:Found", "User not found");
        //    }

        //    var result = await _userManager.ConfirmEmailAsync(user, token);

        //    if (!result.Succeeded)
        //    {
        //        return Error.Unexpected(
        //            "User:Email:Confirmation:Failed",
        //            $"Failed to confirm email for user '{UtilityService.MaskEmail(email)}': {string.Join(", ", result.Errors.Select(e => e.Description))}"
        //        );
        //    }

        //    return Result.Updated;
        //}





        //public async Task<Result<Updated>> ChangePasswordAsync(
        //    string userId,
        //    string currentPassword,
        //    string newPassword,
        //    CancellationToken ct = default
        //)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);

        //    if (user is null)
        //    {
        //        return Error.NotFound("User:Not:Found", "User not found");
        //    }

        //    var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        //    if (!result.Succeeded)
        //    {
        //        return Error.Unexpected(
        //            "Password:Change:Failed",
        //            $"Failed to change password for user '{UtilityService.MaskEmail(user.Email!)}': {string.Join(", ", result.Errors.Select(e => e.Description))}"
        //        );
        //    }

        //    return Result.Updated;
        //}


        //public async Task<Result<IEnumerable<AppUserDto>>> GetUsersAsync(
        //    CancellationToken ct = default
        //)
        //{
        //    var users = await _userManager
        //        .Users.Select(x => new AppUserDto(
        //            x.Id,
        //            x.Email!,
        //            new List<string>(),
        //            new List<Claim>()
        //        ))
        //        .ToListAsync(ct);

        //    return users;
        //}

        //public async Task<Result<AppUserDto>> GetUserByIdAsync(
        //    string userId,
        //    CancellationToken ct = default
        //)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);

        //    if (user is null)
        //    {
        //        return Error.NotFound("User:Not:Found", "User not found");
        //    }

        //    var roles = await _userManager.GetRolesAsync(user);
        //    var claims = await _userManager.GetClaimsAsync(user);

        //    return new AppUserDto(user.Id, user.Email!, roles, claims);
        //}

        public async Task<Result<AppUserDto>> GetUserByEmailAsync(
            string email,
            CancellationToken ct = default
        )
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return Error.NotFound("User:Not:Found", "User not found");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            return new AppUserDto(user.Id, user.FirstName, user.LastName, user.Email!, roles, claims);
        }

        //public async Task<Result<Updated>> UpdateAsync(
        //    AppUserDto user,
        //    CancellationToken ct = default
        //)
        //{
        //    var existingUser = await _userManager.FindByIdAsync(user.UserId);

        //    if (existingUser is null)
        //    {
        //        return Error.NotFound("User:Not:Found", "User not found");
        //    }

        //    existingUser.Email = user.Email;
        //    existingUser.UserName = user.Email;

        //    var result = await _userManager.UpdateAsync(existingUser);

        //    if (!result.Succeeded)
        //    {
        //        return Error.Unexpected(
        //            "User:Update:Failed",
        //            $"Failed to update user '{UtilityService.MaskEmail(existingUser.Email!)}': {string.Join(", ", result.Errors.Select(e => e.Description))}"
        //        );
        //    }

        //    return Result.Updated;
        //}

        //public async Task<Result<Deleted>> DeleteUserAsync(
        //    string userId,
        //    CancellationToken ct = default
        //)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);

        //    if (user is null)
        //    {
        //        return Error.NotFound("User:Not:Found", "User not found");
        //    }

        //    var result = await _userManager.DeleteAsync(user);

        //    if (!result.Succeeded)
        //    {
        //        return Error.Unexpected(
        //            "User:Deletion:Failed",
        //            $"Failed to delete user '{UtilityService.MaskEmail(user.Email!)}': {string.Join(", ", result.Errors.Select(e => e.Description))}"
        //        );
        //    }

        //    return Result.Deleted;
        //}

        //public async Task<Result<Updated>> AssignRoleAsync(
        //    string userId,
        //    string role,
        //    CancellationToken ct = default
        //)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);

        //    if (user is null)
        //    {
        //        return Error.NotFound("User:Not:Found", "User not found");
        //    }

        //    var result = await _userManager.AddToRoleAsync(user, role);

        //    if (!result.Succeeded)
        //    {
        //        return Error.Unexpected(
        //            "User:Role:Assignment:Failed",
        //            $"Failed to assign role '{role}' to user '{UtilityService.MaskEmail(user.Email!)}': {string.Join(", ", result.Errors.Select(e => e.Description))}"
        //        );
        //    }

        //    return Result.Updated;
        //}


    }
}
