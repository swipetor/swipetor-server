using JetBrains.Annotations;
using SwipetorApp.Services.Config.UIConfigs;
using WebLibServer.Config;

namespace SwipetorApp.Services.Config;

[UsedImplicitly]
public class AppConfig
{
    [OutputConfigToUI]
    public SiteConfig Site { get; set; }

    public HostMasterConfig HostMaster { get; set; }

    public SMTPConfig SMTP { get; set; }

    public InfluxConfig Influx { get; set; }

    [OutputConfigToUI]
    public FirebaseConfig Firebase { get; set; }

    public R2Config R2 { get; set; }

    [OutputConfigToUI]
    public RecaptchaConfig Recaptcha { get; set; }

    [OutputConfigToUI]
    public StorageConfig Storage { get; set; }
}