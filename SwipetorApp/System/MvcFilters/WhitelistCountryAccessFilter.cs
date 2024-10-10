using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using SwipetorApp.Services.Config;
using WebAppShared.Contexts;

namespace SwipetorApp.System.MvcFilters;

[UsedImplicitly]
public class WhitelistCountryAccessFilter(
    IHttpContextAccessor httpCtx,
    IConnectionCx ipAddr,
    IOptions<AppConfig> config)
    : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // if (_config.Value.Site.AllowedCountries?.Length > 0 && !_config.Value.Site.AllowedCountries.Contains(_ipAddr.IpAddressCountry.ToUpper()))
        // {
        // 	context.Result = new ContentResult()
        // 	{
        // 		Content = "Hello world"
        // 	};
        // 	return;
        // }

        base.OnActionExecuting(context);
    }
}