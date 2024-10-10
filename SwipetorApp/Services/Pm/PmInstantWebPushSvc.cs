using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.WebPush;
using WebAppShared.Utils;
using WebAppShared.WebSys.DI;

namespace SwipetorApp.Services.Pm;

[Service]
[UsedImplicitly]
public class PmInstantWebPushSvc(WebPushSvc webPushSvc, IDbProvider dbProvider)
{
    public async Task TrySendForPmMsgId(long pmMsgId)
    {
        PmMsg msg;
        await using (var db = dbProvider.Create())
        {
            msg = db.PmMsgs
                .Include(m => m.User)
                .Include(m => m.Thread)
                .ThenInclude(m => m.ThreadUsers).ThenInclude(m => m.User).ThenInclude(u => u.PushDevices)
                .Single(m => m.Id == pmMsgId);
        }

        if (msg == null) throw new Exception("WebPush's TrySendForPmMsgId's msg is null");

        var receiverUsers = msg.Thread.ThreadUsers.Where(tu => tu.UserId != msg.UserId).ToList();

        var pushDevices = receiverUsers.SelectMany(ru => ru.User.PushDevices).ToList();

        if (pushDevices.Count == 0) return;

        var payloads = pushDevices.Select(d => new WebPushPayload
        {
            UserId = msg.UserId,
            PushDevice = d,
            Title = $"{msg.User.Username} sent a new PM",
            Body = msg.Txt.StripHtmlTags().Shorten(180),
            Url = "/pm",
            Icon = "/public/images/label/label-dot-256.png",
            Tag = WebPushTag.NewPmMsg
        }).ToList();

        _ = Task.Run(() =>
        {
            using var db = dbProvider.Create();
            db.PushDevices.Where(pd => pushDevices.Select(p => p.Id).Contains(pd.Id))
                .UpdateFromQuery(pd => new
                {
                    LastUsedAt = DateTime.UtcNow
                });
        });

        await webPushSvc.PushToDevices(payloads);
    }
}