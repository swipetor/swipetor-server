using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Utils;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services.WebPush.Notifs;

[Service]
[UsedImplicitly]
public class NotifWebPushQuerier(IDbProvider dbProvider)
{
    private readonly Expression<Func<Notif, bool>> _notifQueryClause =
        n => n.PushNotifSentAt == null &&
             n.CreatedAt < DateTime.UtcNow.AddDays(-1) &&
             n.IsRead == false &&
             n.IsViewed == false && n.ReceiverUser.PushDevices.Count != 0;

    public readonly Dictionary<int, List<Notif>> NotifsByUserId = new();

    public MultiValueDictionary<int, PushDevice> PushDevicesByUserId;

    public List<int> UserIds { get; private set; }
    public List<Notif> Notifs { get; private set; }

    /// <summary>
    ///     Get userIds that can get notifs and their notifications
    /// </summary>
    public void Run()
    {
        QueryUserIds();
        QueryPushDeviceIds();
        QueryNotifsByUsers();
    }

    /// <summary>
    ///     Returns user notifications to send via push notifications by userId
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, List<Notif>> GetNotifsByUser()
    {
        if (NotifsByUserId.Count > 0) return NotifsByUserId;

        // Populate notifsByuserId
        foreach (var n in Notifs)
        {
            if (!NotifsByUserId.ContainsKey(n.ReceiverUserId))
                NotifsByUserId.Add(n.ReceiverUserId, new List<Notif>());

            NotifsByUserId[n.ReceiverUserId].Add(n);
        }

        return NotifsByUserId;
    }

    /// <summary>
    ///     Set all the notifications as sent via push notification
    /// </summary>
    public void SetNotifsWithPushedAt()
    {
        using var db = dbProvider.Create();

        var notifIds = Notifs.Select(n => n.Id).ToList();

        db.Notifs.Where(n => notifIds.Contains(n.Id)).UpdateFromQuery(n => new
        {
            PushNotifSentAt = DateTime.UtcNow
        });
    }

    /// <summary>
    ///     Get users who can be pushed notification to
    /// </summary>
    private void QueryUserIds()
    {
        using var db = dbProvider.Create();

        // Get 50 user IDs we can push notifications to
        UserIds = db.Notifs.Where(_notifQueryClause).GroupBy(n => n.ReceiverUserId).Take(50)
            .Select(n => n.Key).ToList();
    }

    private void QueryPushDeviceIds()
    {
        using var db = dbProvider.Create();
        var pds = db.PushDevices.Where(pd => UserIds.Contains(pd.UserId)).ToList();
        PushDevicesByUserId = pds.ToMultiValueDictionary(k => k.UserId, v => v);
    }

    /// <summary>
    ///     Get notifications of the users we can push
    /// </summary>
    /// <exception cref="Exception"></exception>
    private void QueryNotifsByUsers()
    {
        if (UserIds == null) throw new Exception("QueryUserIds should be called first.");

        using var db = dbProvider.Create();

        // Get users notifications
        Notifs = db.Notifs.Where(_notifQueryClause).Where(n =>
                UserIds.Contains(n.ReceiverUserId))
            .Include(n => n.ReceiverUser)
            .ToList();
    }
}