using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Auth;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.Emailing;
using SwipetorApp.System.UserRoleAuth;
using WebAppShared.Metrics;

namespace SwipetorApp.Areas.Api;

[ApiController]
[Area(AreaNames.Api)]
[Route("api/email")]
public class EmailApi(EmailerProvider emailerProvider, IDbProvider dbProvider, IMetricsSvc metricsSvc)
    : Controller
{
    [UserRoleAuth(UserRole.HostMaster)]
    [HttpGet("test")]
    public async Task<IActionResult> Test(string to)
    {
        using var emailer = emailerProvider.GetGenericEmailer<string>();
        await emailer.Send(to, "this is a test email", "");

        return Ok();
    }
    
    [HttpGet("unsubscribe/pms")]
    public IActionResult UnsubscribePms(string email, string secret)
    {
        email = email.Trim();

        using var db = dbProvider.Create();

        var user = db.Users.Where(u => u.Email == email && u.Secret == secret).FirstOrDefault();

        if (user == null) return Redirect($"/?msgCode={SayMsgKey.UnsubscribeLinkWrong}");

        user.PmEmailIntervalHours = null;
        db.SaveChanges();

        metricsSvc.Collect("PmEmailUnsubscribed", 1);

        return Redirect($"/?msgCode={SayMsgKey.UnsubscribePmSuccessful}");
    }

    [HttpGet("unsubscribe/notifs")]
    public IActionResult UnsubscribeNotifs(string email, string secret)
    {
        email = email.Trim();

        using var db = dbProvider.Create();

        var user = db.Users.Where(u => u.Email == email && u.Secret == secret).FirstOrDefault();

        if (user == null) return Redirect($"/?msgCode={SayMsgKey.UnsubscribeLinkWrong}");

        user.NotifEmailIntervalHours = null;
        db.SaveChanges();

        metricsSvc.Collect("NotifEmailUnsubscribed", 1);

        return Redirect($"/?msgCode={SayMsgKey.UnsubscribeNotifSuccessful}");
    }

}