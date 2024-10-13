using System;
using System.Linq;
using JetBrains.Annotations;
using SwipetorApp.Services.Contexts;
using WebLibServer.Contexts;
using WebLibServer.Exceptions;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services.Auth;

[Service]
[UsedImplicitly]
public class AuthProtectionSvc(IDbProvider dbProvider, IConnectionCx connectionCx)
{
    public void ThrowIfLoginRequestIdAttemptedBefore(Guid loginRequestId)
    {
        using var db = dbProvider.Create();

        if (db.LoginAttempts.Count(lt => lt.LoginRequestId == loginRequestId) >= 2)
            throw new HttpJsonError("Too many login attempts. Get another code.");
    }

    public void ThrowIfManyLoginAttemptsToUserId(int? userId)
    {
        if (userId == null) return;

        using var db = dbProvider.Create();

        if (db.LoginAttempts.Count(lt => lt.UserId == userId && lt.CreatedAt > DateTime.UtcNow.AddMinutes(-5)) >= 2)
            throw new HttpJsonError("Too many attempts to this account. Wait a few minutes");

        if (db.LoginAttempts.Count(lt => lt.UserId == userId && lt.CreatedAt > DateTime.UtcNow.AddHours(-1)) >= 6)
            throw new HttpJsonError("Too many attempts to this account. Wait an hour.");

        if (db.LoginAttempts.Count(
                lt => lt.UserId == userId && lt.CreatedAt > DateTime.UtcNow.AddDays(-1)) >= 10)
            throw new HttpJsonError("Too many attempts to this account. Come back tomorrow.");
    }

    public void ThrowIfManyLoginAttemptsFromThisIpAddress()
    {
        using var db = dbProvider.Create();

        if (db.LoginAttempts.Count(
                lt => lt.CreatedIp == connectionCx.IpAddress && lt.CreatedAt > DateTime.UtcNow.AddDays(-1)) >= 20)
            throw new HttpJsonError("Too many login attempts from this IP address. Come back tomorrow.");

        // If the IP address block was already tried 100+ times within the last 24 hours
        var ipAddressBlocks = connectionCx.IpAddress.Split('.');
        var firstThreeBlocks = string.Join(".", ipAddressBlocks.Take(3)); // Takes the first three blocks

        if (db.LoginAttempts.Count(l =>
                l.CreatedIp.StartsWith(firstThreeBlocks) && l.CreatedAt > DateTime.UtcNow.AddHours(-24)) > 100)
            throw new HttpJsonError("This IP block tried too many logins. Please try again tomorrow.");
    }
}