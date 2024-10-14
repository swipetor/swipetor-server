using System;
using System.ComponentModel;
using System.Globalization;
using JetBrains.Annotations;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Config.UIConfigs;
using WebLibServer.SharedLogic.Fx;

namespace SwipetorApp.Services.Config;

[UsedImplicitly]
public class SiteConfig
{
    [OutputConfigToUI]
    public string Name { get; set; }

    [OutputConfigToUI]
    public string BaseDomain { get; set; }

    [OutputConfigToUI]
    public string Hostname { get; set; }

    [OutputConfigToUI]
    public string HostPort { get; set; }

    [OutputConfigToUI]
    public string Email { get; set; }

    [OutputConfigToUI]
    public string Slogan { get; set; }
    
    [OutputConfigToUI]
    public string UserProfileTitle { get; set; }

    [OutputConfigToUI]
    public string UserProfileDesc { get; set; }

    [OutputConfigToUI]
    public string Description { get; set; }

    public Currency DefaultCurrency { get; set; } = Currency.EUR;

    [OutputConfigToUI]
    public bool IsRta { get; set; }
}
