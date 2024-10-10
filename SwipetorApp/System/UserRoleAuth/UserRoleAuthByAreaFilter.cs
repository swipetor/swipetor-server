using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using SwipetorApp.Services.Auth;

namespace SwipetorApp.System.UserRoleAuth;

public class UserRoleAuthByAreaFilter(UserRole minRole, string areaToApply) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var area = (context.RouteData.Values["area"] as string)?.ToLowerInvariant();
        if (area == areaToApply?.ToLowerInvariant())
            // Check if the UserRoleAuth attribute is already present
            if (!context.Filters.Any(f => f is UserRoleAuthAttribute))
            {
                var attribute = new UserRoleAuthAttribute(minRole);
                attribute.OnAuthorization(context);
            }
    }
}