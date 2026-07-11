using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Loqora.Application.Common.Interfaces;

namespace Loqora.Api.Services
{
    public class CurrentUser(IHttpContextAccessor contextAccessor) : IUser
    {
        private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

        public string? Id =>
            _contextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Sub);
    }
}
