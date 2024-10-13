using JetBrains.Annotations;
using SwipetorApp.Services.Config.UIConfigs;
using WebLibServer.SharedLogic.Recaptcha;

namespace SwipetorApp.Services.Config;

[UsedImplicitly]
public class RecaptchaConfig : IRecaptchaConfig
{
    [OutputConfigToUI]
    public string Key { get; set; }

    public string Secret { get; set; }
}