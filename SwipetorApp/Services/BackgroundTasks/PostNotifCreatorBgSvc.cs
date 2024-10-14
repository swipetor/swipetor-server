using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.Notifs;
using WebLibServer.BackgroundTasks;
using WebLibServer.DI;
using WebLibServer.Metrics;

namespace SwipetorApp.Services.BackgroundTasks;

public class PostNotifCreatorBgSvc(
    ILogger<PostNotifCreatorBgSvc> logger,
    IMetricsSvc metricsSvc,
    IServiceProvider serviceProvider)
    : CronBase(logger, metricsSvc)
{
    protected override int DurationInMinutes => 1;

    protected override async Task RunAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        
        var notifSvc = scope.ServiceProvider.GetRequiredService<NotifSvc>();
            
        int totalCreatedCount = 0;
        int loopCount = 0;

        while (true)
        {
            var createdCount = await notifSvc.GetNotifsToCreate();
            totalCreatedCount += createdCount;
            loopCount++;

            if (loopCount > 100)
            {
                metricsSvc.CollectNamed(AppEvent.CriticalException);
                throw new Exception("Looped more than 100 times in creating notifications.");
            }

            if (createdCount == 0 || totalCreatedCount >= 100)
                break;
        }
        
        metricsSvc.CollectNamed(AppEvent.NotifPostCreated, totalCreatedCount);
    }
}