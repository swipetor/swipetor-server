using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.EmailViewModels;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.Emailing;
using WebAppShared.Contexts;
using WebAppShared.Exceptions;
using WebAppShared.Metrics;
using WebAppShared.WebSys.DI;

namespace SwipetorApp.Services.Auth;

[Service]
[UsedImplicitly]
public class LoginCodeSvc(
    IDbProvider dbProvider,
    IConnectionCx connectionCx,
    EmailerProvider emailerProvider,
    IMetricsSvc metricsSvc)
{
    /// <summary>
    ///     This method does various checks to see if the user is allowed to proceed with the login link request.
    ///     We want to prevent spamming of login links.
    /// </summary>
    /// <param name="email"></param>
    /// <exception cref="HttpJsonError"></exception>
    public async Task ThrowIfNotAllowedToProceed(string email)
    {
        await using var db = dbProvider.Create();

        // If the email address was already tried within the last 5 minutes
        if (db.LoginRequests.Count(l => l.Email == email && l.CreatedAt > DateTime.UtcNow.AddMinutes(-5)) >= 2)
            throw new HttpJsonError("Please wait a few minutes before requesting another login link.");

        // If the email address was already tried 5+ times within the last 60 minutes
        if (db.LoginRequests.Count(l =>
                l.CreatedIp == connectionCx.IpAddress && l.CreatedAt > DateTime.UtcNow.AddMinutes(-60)) > 5)
            throw new HttpJsonError("This IP Address tried too many logins. Please wait about an hour.");

        // If the IP address was already tried 20+ times within the last 24 hours
        if (db.LoginRequests.Count(l =>
                l.CreatedIp == connectionCx.IpAddress && l.CreatedAt > DateTime.UtcNow.AddHours(-24)) > 20)
            throw new HttpJsonError("This IP Address tried too many logins. Please try again tomorrow.");

        // If the IP address block was already tried 100+ times within the last 24 hours
        var ipAddressBlocks = connectionCx.IpAddress.Split('.');
        var firstThreeBlocks = string.Join(".", ipAddressBlocks.Take(3)); // Takes the first three blocks

        if (db.LoginRequests.Count(l =>
                l.CreatedIp.StartsWith(firstThreeBlocks) && l.CreatedAt > DateTime.UtcNow.AddHours(-24)) > 100)
            throw new HttpJsonError("This IP block tried too many logins. Please try again tomorrow.");
    }

    public async Task EmailLoginCode(LoginRequest loginRequest)
    {
        await using var db = dbProvider.Create();

        var username = db.Users.Where(u => u.Email == loginRequest.Email).Select(u => u.Username).FirstOrDefault();

        using var loginCodeEmailer = emailerProvider.GetLoginCodeEmailer();
        await loginCodeEmailer.Send(loginRequest.Email, "Your requested login code", new LoginCodeEmailVM
        {
            Username = username,
            LoginRequest = loginRequest
        });

        metricsSvc.Collect("LoginLinkEmailSent", 1);
    }
}