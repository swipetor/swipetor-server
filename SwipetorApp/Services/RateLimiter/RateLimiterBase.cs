using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using WebAppShared.Contexts;
using WebAppShared.Exceptions;
using NotImplementedException = System.NotImplementedException;

namespace SwipetorApp.Services.RateLimiter;

public abstract class RateLimiterBase(IConnectionCx connectionCx, ILogger logger) : IRateLimiter
{
    protected virtual List<RateLimit> RateLimits => throw new NotImplementedException();

    protected void EvaluateTimestamps(List<DateTime> timestamps)
    {
        DateTime now = DateTime.UtcNow;

        foreach (var limit in RateLimits)
        {
            var count = timestamps.Count(time => now - time <= limit.TimeSpan);
            if (count >= limit.Limit)
            {
                logger.LogWarning("Rate limit exceeded for IP address {IpAddress}, count {Count} in minutes {Time}",
                    connectionCx.IpAddress, count, limit.TimeSpan.TotalMinutes);
                throw new HttpJsonError("Rate limit exceeded. Try again later.");
            }
        }
    }
    
    protected DateTime OldestRateLimitTime()
    {
        DateTime now = DateTime.UtcNow;
        // Determine the longest duration to query once
        var longestDuration = RateLimits.Max(r => r.TimeSpan);
        return now - longestDuration;
    }

    public abstract void Run();
}