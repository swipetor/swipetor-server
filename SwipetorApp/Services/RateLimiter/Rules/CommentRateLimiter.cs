using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using SwipetorApp.Services.Contexts;
using WebAppShared.Contexts;
using WebAppShared.WebSys.DI;

namespace SwipetorApp.Services.RateLimiter.Rules;

[Service]
[UsedImplicitly]
public class CommentRateLimiter(
    IDbProvider dbProvider,
    ILogger<PhotoUploadRateLimiter> logger,
    IConnectionCx connectionCx,
    UserIdCx userIdCx) : RateLimiterBase(connectionCx, logger)
{
    protected override List<RateLimit> RateLimits =>
    [
        new RateLimit(3, TimeSpan.FromMinutes(1)),
        new RateLimit(20, TimeSpan.FromHours(1))
    ];

    public override void Run()
    {
        logger.LogInformation("Running rate limiter check for IP address {IpAddress}", connectionCx.IpAddress);
        using var db = dbProvider.Create();

        var oldestTime = OldestRateLimitTime();
        var timestamps = db.Comments
            .Where(c => c.CreatedAt >= oldestTime &&
                        (c.CreatedIp == connectionCx.IpAddress || c.UserId == userIdCx.Value))
            .Select(c => c.CreatedAt)
            .ToList();

        EvaluateTimestamps(timestamps);
    }
}