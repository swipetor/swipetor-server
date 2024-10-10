using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Config;
using SwipetorApp.Services.Contexts;
using WebAppShared.Metrics;
using WebAppShared.Utils;
using WebAppShared.WebSys.DI;
using WebAppShared.WebSys.Exceptions;

namespace SwipetorApp.Services.Auth;

[Service]
[UsedImplicitly]
public class AuthSvc(
    IHttpContextAccessor hca,
    IMetricsSvc metricsSvc,
    IWebHostEnvironment webHostEnv,
    IOptions<SiteConfig> siteConfig,
    IDbProvider dbProvider)
{
    public User GetOrCreateUser(string email)
    {
        email = email.Trim();

        using var db = dbProvider.Create();

        var user = db.Users.Where(u => EF.Functions.ILike(u.Email, email)).SingleOrDefault();

        if (user == null)
        {
            user = new User
            {
                Email = email,
                Secret = StringUtils.RandomString(16)
            };

            db.Users.Add(user);
            db.SaveChanges();
        }

        return user;
    }

    /// <summary>
    ///     Logs in the given user no matter what
    /// </summary>
    /// <param name="user"></param>
    /// <exception cref="GlobalErrorException"></exception>
    public async Task LoginWith(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Sid, user.Id.ToString()),
            new(ClaimTypes.Hash, user.Secret),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var claimsId = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProps = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(24),
            AllowRefresh = true,
            IssuedUtc = DateTime.UtcNow
        };
        
        // Set secure cookie settings for production environment
        if (webHostEnv.IsProduction())
        {
            authProps.Items[".Cookie.SameSite"] = "Strict";
            authProps.Items[".Cookie.Secure"] = "true";
            authProps.Items[".Cookie.Domain"] = siteConfig.Value.BaseDomain;
        }

        if (hca.HttpContext == null)
            throw new Exception("httpContext is null in AuthCommon, it shouldn't be.");

        metricsSvc.CollectNamed(AppEvent.UserLogin, 1);

        await hca.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsId), authProps);
    }
}