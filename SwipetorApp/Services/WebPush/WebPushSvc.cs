using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MoreLinq;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Services.Contexts;
using WebAppShared.WebPush;
using WebAppShared.WebSys.DI;

namespace SwipetorApp.Services.WebPush;

[Service]
[UsedImplicitly]
public class WebPushSvc(ILoggerFactory loggerFactory, IDbProvider dbProvider, UserIdCx userIdCx)
    : WebPushSvcBase(loggerFactory.CreateLogger<WebPushSvcBase>())
{
    private readonly ILogger<WebPushSvc> _logger = loggerFactory.CreateLogger<WebPushSvc>();

    protected override void CleanupFailedPushDeviceIdsInDb(List<int> pushDeviceIdsToRemove)
    {
        using var db = dbProvider.Create();
        int count = db.PushDevices.Where(pd => pushDeviceIdsToRemove.Contains(pd.Id)).DeleteFromQuery();
        _logger.LogInformation("WEB_PUSH_CLEANUP2: pushDeviceIds count:{Count} (should be) removed: {Ids}",
            count, pushDeviceIdsToRemove.ToDelimitedString(","));
    }

    public PushDevice SaveGetPushDevice(string token)
    {
        using var db = dbProvider.Create();

        var pushDevice = db.PushDevices.SingleOrDefault(p => p.Token == token && p.UserId == userIdCx.Value);

        if (pushDevice != null) return pushDevice;

        pushDevice = new PushDevice
        {
            UserId = userIdCx.Value,
            Token = token,
            LastUsedAt = DateTime.UtcNow
        };

        db.PushDevices.Add(pushDevice);
        db.SaveChanges();

        return pushDevice;
    }
}