using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace SwipetorApp.Services.Config;

[UsedImplicitly]
public class R2Config
{
    [MaxLength(128)]
    public string Key { get; set; }

    [MaxLength(256)]
    public string Secret { get; set; }

    [MaxLength(512)]
    public string ServiceUrl { get; set; }
}