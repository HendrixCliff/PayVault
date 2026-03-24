using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt; 

namespace PayVault.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                      ?? user.FindFirst("userId")?.Value;
            
            return Guid.Parse(userId ?? Guid.Empty.ToString());
        }

        public static string GetUserRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value ?? "User";
        }
    }
}