using System;
using System.Linq;
using JetBrains.Annotations;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.Notifs;
using WebLibServer.Contexts;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services.Referring;

[Service]
[UsedImplicitly]
public class ReferrerSvc(IConnectionCx connCx, UserCx userCx, IDbProvider dbProvider, NotifSvc notifSvc)
{
    public void ProcessReferrer(int referrerId)
    {
        using var db = dbProvider.Create();

        var referrer = db.Users.Where(rid => rid.Id == referrerId).SingleOrDefault();

        if (referrer == null) return;

        if (connCx.IpAddress == referrer.LastOnlineIp)
            return;
        
        // If got premium again this week, skip
        if (referrer.PremiumUntil > DateTime.UtcNow.AddDays(6))
            return;
        
        // If this user is logged in an same as referrer, skip
        if (referrer.Id == userCx.ValueOrNull?.Id) return;
        
        referrer.PremiumUntil = DateTime.UtcNow.AddDays(7);
        db.SaveChanges();
        
        notifSvc.NewReferralPremium(referrer.Id);
    }
}