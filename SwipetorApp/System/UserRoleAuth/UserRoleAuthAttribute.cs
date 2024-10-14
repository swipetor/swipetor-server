using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SwipetorApp.Services.Auth;
using SwipetorApp.Services.Config;
using SwipetorApp.Services.Contexts;
using WebLibServer.Utils;

namespace SwipetorApp.System.UserRoleAuth;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class UserRoleAuthAttribute(UserRole requiredRole) : Attribute, IAuthorizationFilter
{
    private readonly int _requiredRole = (int)requiredRole;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // If auth header is there, that's all what matters
        var roleFromAuthHeader = RoleFromAuthHeader(context);
        if (roleFromAuthHeader != null)
        {
            if ((int)roleFromAuthHeader >= _requiredRole) return; // authorized
            // Otherwise, not authorized.
            context.Result = new ForbidResult();
            return;
        }

        // There is no Auth header present, try to authorize by the logged in user
        var userCx = context.HttpContext.RequestServices.GetService<UserCx>();
        
        if (!userCx.IsLoggedIn)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // assign value of userCx to int
        var currentUserRoleInt = (int)userCx.Value.Role;

        // If the user is not authorized, set the context.Result to a ForbidResult().
        // You can also use UnauthorizedResult() based on your needs.
        if (currentUserRoleInt < _requiredRole) context.Result = new ForbidResult();
    }

    /// <summary>
    ///     Check if the user is authorized by the Authorization header.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="userCx"></param>
    /// <returns></returns>
    private static UserRole? RoleFromAuthHeader(AuthorizationFilterContext context)
    {
        var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ")) return null;

        var hostmasterBearerSha256 = context.HttpContext.RequestServices.GetService<IOptions<HostMasterConfig>>().Value
            .MasterBearer.Trim();
        var usersBearer = authorizationHeader.Substring("Bearer ".Length).Trim();
        var usersBearerSha256 = PasswordUtils.Sha256Hash(usersBearer);

        if (hostmasterBearerSha256 == usersBearerSha256)
            return UserRole.HostMaster;

        return null;
    }
}