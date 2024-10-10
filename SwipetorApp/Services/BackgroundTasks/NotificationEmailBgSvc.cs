using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SwipetorApp.Models.EmailViewModels;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.Emailing;
using WebAppShared.BackgroundTasks;
using WebAppShared.Metrics;
using User = SwipetorApp.Models.DbEntities.User;

namespace SwipetorApp.Services.BackgroundTasks;

public class NotificationEmailBgSvc(
    ILogger<NotificationEmailBgSvc> logger,
    EmailerProvider emailProvider,
    IMetricsSvc metricsSvc,
    IDbProvider dbProvider)
    : CronBase(logger, metricsSvc)
{
    protected override int DurationInMinutes => 5;

    protected override async Task RunAsync(CancellationToken stoppingToken)
    {
        try
        {
            List<User> usersToEmail;
            await using (var db = dbProvider.Create())
            {
                usersToEmail = db.Users.Where(u =>
                        u.NotifEmailIntervalHours != null &&
                        u.Notifs.Any(n => n.IsViewed == false) &&
                        u.LastOnlineAt < DateTime.UtcNow.AddHours(-1 * u.NotifEmailIntervalHours.Value) &&
                        u.LastNotifEmailAt <= u.LastNotifCheckAt)
                    .OrderBy(u => u.LastNotifEmailAt)
                    .Take(10).ToList();
            }

            // Send InfluxDB analytics
            metricsSvc.CollectNamed(AppEvent.NotifEmailToUsers, usersToEmail.Count);

            if (usersToEmail.Count > 0)
                logger.LogInformation("Found {Count} users to send notification email to", usersToEmail.Count);

            foreach (var u in usersToEmail)
            {
                // Update user's last notification email at
                // Use using statement to reduce the connection time frame
                await using (var gcx = dbProvider.Create())
                {
                    gcx.Users.Attach(u);
                    u.LastNotifEmailAt = DateTime.UtcNow;
                    await gcx.SaveChangesAsync(stoppingToken);
                }

                var notifModel = new NotifEmailVM
                {
                    User = u
                };
                logger.LogInformation("Sending notification email for user {Username} {Id}", u.Username, u.Id);

                using (var emailer = emailProvider.GetNotifEmailer(u))
                {
                    await emailer.Send(u.Email,
                        "You have unread notifications", notifModel);
                }
            }
        }
        catch (OperationCanceledException)
        {
            metricsSvc.CollectNamed(BaseAppEvent.CriticalException, 1);
            // catch the cancellation exception
            // to stop execution
        }
        catch (Exception ex)
        {
            metricsSvc.CollectNamed(BaseAppEvent.CriticalException, 1);
            logger.LogError(ex, "Exception while sending Notification Emails in background: {Msg}", ex.Message);
        }
    }
}

