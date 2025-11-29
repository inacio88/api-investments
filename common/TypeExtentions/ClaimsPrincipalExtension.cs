using System.Security.Claims;

namespace common.TypeExtentions
{
    public static class ClaimsPrincipalExtension
    {
        public static string? UserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        
    }
}