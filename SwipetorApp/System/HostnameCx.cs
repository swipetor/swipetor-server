using System.Linq;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SwipetorApp.Services.Config;
using SwipetorApp.Services.Contexts;
using WebLibServer.Types;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.System;

[Service]
[UsedImplicitly]
public class HostnameCx(IOptions<SiteConfig> siteConfig, IHttpContextAccessor httpContextAccessor) : IHostnameCx
{
    public string SiteHostname => GetSiteHostname();
    public string SiteHostnameWithoutPort => GetSiteHostname(false);
    public string RequestedHostname => GetRequestedHostname();
    
    private string GetRequestedHostname()
    {
        return httpContextAccessor.HttpContext?.Request?.Host.Value;
    }
    
    private string GetSiteHostname(bool includePort = true)
    {
        var h = siteConfig.Value.Hostname;
        
        if (includePort && !string.IsNullOrWhiteSpace(siteConfig.Value.HostPort))
            h += ":" + siteConfig.Value.HostPort;

        return h;
    }
}
