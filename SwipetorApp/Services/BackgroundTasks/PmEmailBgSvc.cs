using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.EmailViewModels;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.Emailing;
using WebLibServer.BackgroundTasks;
using WebLibServer.Metrics;
using WebLibServer.Utils;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services.BackgroundTasks;

public class PmEmailBgSvc(
    ILogger<PmEmailBgSvc> logger,
    EmailerProvider emailerProvider,
    IMetricsSvc metricsSvc,
    IDbProvider dbProvider)
    : CronBase(logger, metricsSvc)
{
    protected override int DurationInMinutes => 10;

    protected override async Task RunAsync(CancellationToken stoppingToken)
    {
        try
        {
            List<PmThreadUser> pmThreadUsersToEmail;
            await using (var db = dbProvider.Create())
            {
                pmThreadUsersToEmail = db.PmThreadUsers
                    .Include(u => u.User)
                    .Include(tu => tu.Thread).ThenInclude(t => t.ThreadUsers).ThenInclude(tux => tux.User)
                    .Where(tu =>
                        tu.EmailSentAt ==
                        null && // No email was sent for this thread yet. When email is sent, it's updated. When thread is read, it's set to null again. 
                        tu.UnreadMsgCount > 0 && // There are unread messages
                        tu.User.PmEmailIntervalHours != null && // User accepts PM reminder emails
                        // User checked PMs before PM Email Interval
                        tu.User.LastPmCheckAt < DateTime.UtcNow.AddHours(tu.User.PmEmailIntervalHours.Value) &&
                        tu.FirstUnreadMsg.CreatedAt < DateTime.UtcNow.AddHours(tu.User.PmEmailIntervalHours.Value))
                    .OrderBy(tu => tu.LastMsgAt)
                    .Take(50)
                    .ToList();
                // Update EmailSentAt
                pmThreadUsersToEmail.ForEach(tu => tu.EmailSentAt = DateTime.UtcNow);
                await db.SaveChangesAsync(stoppingToken);
            }

            metricsSvc.CollectNamed(AppEvent.PmEmailUsers, pmThreadUsersToEmail.Count);

            foreach (var tu in pmThreadUsersToEmail)
            {
                var otherUserNames = string.Join(",",
                    tu.Thread.ThreadUsers.Where(all => all.UserId != tu.UserId)
                        .Select(all => all.User.Username).ToList().Select(un => un.Shorten(12)).ToList());

                var pmEmailModel = new PmEmailVM
                {
                    PmThreadUser = tu,
                    OtherUsernames = otherUserNames
                };
                logger.LogInformation("Sending PM email for user {Username} #{Id} for thread #{ThreadId}",
                    tu.User.Username,
                    tu.UserId, tu.ThreadId);

                using var emailer = emailerProvider.GetPmEmailer(tu.User);
                await emailer.Send(tu.User.Email,
                    "New private message in thread " + otherUserNames.Shorten(60), pmEmailModel);
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
            logger.LogError(ex, "Exception while sending PM Emails in background: {Msg}", ex.Message);
        }
    }
}