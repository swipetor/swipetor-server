using JetBrains.Annotations;

namespace SwipetorApp.Services.Config;

[UsedImplicitly]
public class HostMasterConfig
{
    public string MasterBearer { get; set; }
    public string Email { get; set; }
}