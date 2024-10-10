using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SwipetorApp.Services.Auth;

namespace SwipetorApp.System.UserRoleAuth;

public class MinimumRoleHandler : AuthorizationHandler<MinimumRoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        MinimumRoleRequirement requirement)
    {
        if (!context.User.HasClaim(c => c.Type == ClaimTypes.Role)) return Task.CompletedTask;

        var roleClaim = context.User.FindFirst(c => c.Type == ClaimTypes.Role);

        if (Enum.TryParse<UserRole>(roleClaim?.Value, out var userRole) && userRole >= requirement.Role)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}