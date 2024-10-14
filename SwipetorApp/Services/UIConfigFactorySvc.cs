using AutoMapper;
using CloneExtensions;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SwipetorApp.Services.Config;
using SwipetorApp.Services.Config.UIConfigs;
using SwipetorApp.Services.Security;
using SwipetorApp.Services.Users;
using WebLibServer.SharedLogic.Fx;
using WebLibServer.SharedLogic.Recaptcha;
using WebLibServer.Utils;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services;

[Service]
[UsedImplicitly]
public class UIConfigFactorySvc(
    IOptions<AppConfig> appConfig,
    IRecaptchaConfig recaptchaConfig,
    DefaultLocationCurrencyCx defaultLocationCurrencyCx)
{
    public string GetUIConfigAsJson()
    {
        var config = appConfig.Value.GetClone();
        config.Site.DefaultCurrency = defaultLocationCurrencyCx;
        config.Site.Email = config.Site.Email.ToBase64();
        
        config.Recaptcha.Key = recaptchaConfig.Key;
        
        var configStr = JsonConvert.SerializeObject(config,
            new JsonSerializerSettings { ContractResolver = new UIConfigContractResolver() });

        return configStr;
    }
}