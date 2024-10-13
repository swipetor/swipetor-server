using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.EmailViewModels;
using SwipetorApp.Services.Config;
using WebLibServer.Emailing;
using WebLibServer.Metrics;
using WebLibServer.MVC;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services.Emailing;

[Service]
[UsedImplicitly]
public class EmailerProvider(
    IViewRenderService viewRenderService,
    IOptions<SiteConfig> siteConfig,
    IOptions<SMTPConfig> smtpConfig,
    IMetricsSvc metricsSvc,
    ILoggerFactory loggerFactory)
{
    private readonly EmailerCx _emailCx = new()
    {
        ViewRenderService = viewRenderService,
        Config = new EmailerConfigBase
        {
            Hostname = siteConfig.Value.Hostname,
            SiteEmail = siteConfig.Value.Email,
            SiteName = siteConfig.Value.Name,
            SmtpPort = smtpConfig.Value.Port,
            SmtpEncryption = smtpConfig.Value.Encryption
        },
        MetricsSvc = metricsSvc
    };

    private ILogger<Emailer<T>> EmailerLogger<T>()
    {
        return loggerFactory.CreateLogger<Emailer<T>>();
    }
    
    public Emailer<PmEmailVM> GetPmEmailer(User user)
    {
        var cx = _emailCx.Clone();
        cx.AddListUnsubscribeHeader(user.Secret);
        cx.EmailType = OutgoingEmailType.Pm;

        return new Emailer<PmEmailVM>(cx, EmailerLogger<PmEmailVM>());
    }

    public Emailer<NotifEmailVM> GetNotifEmailer(User user)
    {
        var cx = _emailCx.Clone();
        cx.AddListUnsubscribeHeader(user.Secret);
        cx.EmailType = OutgoingEmailType.Notif;

        return new Emailer<NotifEmailVM>(cx, EmailerLogger<NotifEmailVM>());
    }

    public Emailer<LoginCodeEmailVM> GetLoginCodeEmailer()
    {
        var cx = _emailCx.Clone();
        cx.EmailType = OutgoingEmailType.LoginCode;

        return new Emailer<LoginCodeEmailVM>(cx, EmailerLogger<LoginCodeEmailVM>());
    }

    public Emailer<T> GetGenericEmailer<T>(Action<EmailerCx> customizeCx = null)
    {
        var cx = _emailCx.Clone();
        cx.EmailType = OutgoingEmailType.Generic;

        if (customizeCx != null) customizeCx(cx);

        return new Emailer<T>(cx, EmailerLogger<T>());
    }
}