using JetBrains.Annotations;
using SwipetorApp.Services.Config.UIConfigs;
using WebLibServer.Uploaders;

namespace SwipetorApp.Services.Config;

[UsedImplicitly]
public class StorageConfig : IStorageConfig
{
    [OutputConfigToUI]
    public string MediaHost { get; set; }

    [OutputConfigToUI]
    public string BucketNamePrefix { get; set; }

    [OutputConfigToUI]
    public string BucketNameSuffix { get; set; }
}