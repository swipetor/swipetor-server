using JetBrains.Annotations;
using SwipetorApp.Services.Config.UIConfigs;

namespace SwipetorApp.Services.Config;

[UsedImplicitly]
public class FirebaseConfig
{
    [OutputConfigToUI]
    public string AppId { get; set; }

    [OutputConfigToUI]
    public string ApiKey { get; set; }

    [OutputConfigToUI]
    public string ProjectId { get; set; }

    [OutputConfigToUI]
    public string AuthDomain { get; set; }
    
    [OutputConfigToUI]
    public string StorageBucket { get; set; }

    [OutputConfigToUI]
    public string MessagingSenderId { get; set; }

    [OutputConfigToUI]
    public string VapidKey { get; set; }
}