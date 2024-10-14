using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using SwipetorApp.Services.Contexts;
using WebLibServer.Contexts;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services.RateLimiter.Rules;

[Service]
[UsedImplicitly]
public class VideoUploadRateLimiter(
    IDbProvider dbProvider,
    ILogger<VideoUploadRateLimiter> logger,
    IConnectionCx connectionCx,
    UserIdCx userIdCx) : RateLimiterBase(connectionCx, logger)
{
    protected override List<RateLimit> RateLimits =>
    [
        new RateLimit(4, TimeSpan.FromMinutes(1)),
        new RateLimit(20, TimeSpan.FromHours(1)),
        new RateLimit(50, TimeSpan.FromHours(24))
    ];

    public override void Run()
    {
        logger.LogInformation("Running rate limiter check for IP address {IpAddress}", connectionCx.IpAddress);
        using var db = dbProvider.Create();
        
        var oldestTime = OldestRateLimitTime();

        var timestamps = db.PostMedias
            .Where(m => m.Video.CreatedAt >= oldestTime &&
                        (m.Video.CreatedIp == connectionCx.IpAddress || m.Post.UserId == userIdCx.Value))
            .Select(v => v.Video.CreatedAt)
            .ToList();

        EvaluateTimestamps(timestamps);
    }
}