using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using SwipetorApp.Services.Config;
using SwipetorApp.Services.Users;
using WebAppShared.SharedLogic.Recaptcha;
using WebAppShared.WebSys.DI;

namespace SwipetorApp.Services.Security;

/// <summary>
/// This is not directly used, use IRecaptchaConfig instead.
/// </summary>
/// <param name="recaptchaConfig"></param>
/// <param name="customDomainSvc"></param>
[Service]
[UsedImplicitly]
public class RecaptchaCredsSvc(IOptions<RecaptchaConfig> recaptchaConfig)
    : IRecaptchaConfig
{
    private RecaptchaConfig _cachedConfig;

    public string Key => Get().Key;
    public string Secret => Get().Secret;

    private RecaptchaConfig Get()
    {
        if (_cachedConfig != null)
            return _cachedConfig;

        var config = recaptchaConfig.Value;
        
        _cachedConfig = config;
        return _cachedConfig;
    }
}