using Microsoft.AspNetCore.Authorization;
using SwipetorApp.Services.Auth;

namespace SwipetorApp.System.UserRoleAuth;

public class MinimumRoleRequirement(UserRole role) : IAuthorizationRequirement
{
    public UserRole Role { get; } = role;
}