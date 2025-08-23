using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Es2al.Services.ExtensionMethods
{
    public static class UserClaimPrincipleExtensions
    {
        public static int GetUserIdAsInt(this ClaimsPrincipal user)
        {
            return Int32.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
        public static string GetUserIdAsString(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }
    }
}
