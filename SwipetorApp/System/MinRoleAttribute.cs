using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SwipetorApp.Services.Auth;
using SwipetorApp.Services.Contexts;

namespace SwipetorApp.System;

public class MinRoleAttribute(UserRole requiredRole) : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userCx = context.HttpContext.RequestServices.GetRequiredService<UserCx>();

        if (!userCx.IsLoggedIn || userCx.ValueOrNull == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (userCx.Value.Role < requiredRole) context.Result = new UnauthorizedResult();
    }
}