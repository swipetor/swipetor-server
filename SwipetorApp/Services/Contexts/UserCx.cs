using System;
using System.Linq;
using System.Security.Claims;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SwipetorApp.Models.DbEntities;
using WebLibServer.Exceptions;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services.Contexts;

[Service(ServiceAttribute.Scopes.Scoped)]
[UsedImplicitly]
[NotNull]
public class UserCx
{
    private readonly IDbProvider _dbProvider;
    private readonly IHttpContextAccessor _hce;
    private readonly Lazy<User> _lazy;

    public UserCx(IHttpContextAccessor hce, IDbProvider dbProvider)
    {
        _hce = hce;
        _dbProvider = dbProvider;
        _lazy = new Lazy<User>(Load);
    }

    private UserCx()
    {
    }

    [NotNull]
    public User Value => _lazy.Value ?? throw new HttpJsonError("User is not logged in");

    [CanBeNull]
    public User ValueOrNull => _lazy.Value;

    public bool IsLoggedIn => _lazy.Value != null;

    [NotNull]
    public static implicit operator User(UserCx value)
    {
        return value.Value;
    }

    private User Load()
    {
        var httpContextUser = _hce.HttpContext?.User;

        // If there is no user
        if (httpContextUser == null) return null;

        var sid = httpContextUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);
        var hash = httpContextUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Hash);

        if (sid?.Value == null || hash?.Value == null)
            return null;

        User user;
        using (var db = _dbProvider.Create())
        {
            user = db.Users.Include(u => u.Photo).AsNoTracking()
                .SingleOrDefault(u => u.Id == int.Parse(sid.Value) && u.Secret == hash.Value);
        }

        if (user == null)
            // Sign out if sid or hash is null to clean them
            _hce.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();

        return user;
    }
}