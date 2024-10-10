using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoreLinq;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.WebPush;
using SwipetorApp.Services.WebPush.Notifs;
using WebAppShared.BackgroundTasks;
using WebAppShared.DI;
using WebAppShared.Metrics;

namespace SwipetorApp.Services.BackgroundTasks;

public class NotifWebPushBgSvc(
    ILogger<NotifWebPushBgSvc> logger,
    IMetricsSvc metricsSvc,
    IServiceProvider serviceProvider)
    : CronBase(logger, metricsSvc)
{
    protected override int DurationInMinutes => 1;

    protected override Task RunAsync(CancellationToken stoppingToken)
    {
        NotifWebPushQuerier querier = null;
        using var scope = serviceProvider.CreateScope();
        var webPushSvc = scope.ServiceProvider.GetRequiredService<WebPushSvc>();
        var metricsSvc = scope.ServiceProvider.GetRequiredService<IMetricsSvc>();
        var notifWebPushQueriesFactory = scope.ServiceProvider.GetRequiredService<IFactory<NotifWebPushQuerier>>();

        try
        {
            querier = notifWebPushQueriesFactory.GetInstance();
            querier.Run();

            var notifsByUserId = querier.GetNotifsByUser();

            metricsSvc.CollectNamed(AppEvent.NotifWebPushUsers, notifsByUserId.Count);

            var pushPayloads = notifsByUserId
                .Select(pair => new NotifWebPushFormatter(pair.Key, pair.Value).Format()).ToList();
            
            var multiDevicePushPayloads = pushPayloads.SelectMany(p => querier.PushDevicesByUserId[p.UserId]
                .Select(pd => new WebPushPayload(p)
                {
                    PushDevice = pd
                })).ToList();

            metricsSvc.CollectNamed(AppEvent.NotifWebPushMultiDevice, multiDevicePushPayloads.Count);

            if (multiDevicePushPayloads.Any())
            {
                async void PushToDevices(IEnumerable<WebPushPayload> b)
                {
                    await webPushSvc.PushToDevices(multiDevicePushPayloads);
                }

                multiDevicePushPayloads.Batch(100).ForEach(PushToDevices);
            }
        }
        finally
        {
            querier?.SetNotifsWithPushedAt();
        }

        return Task.CompletedTask;
    }
}