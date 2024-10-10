using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using SwipetorApp.Services.Config;
using WebAppShared.Uploaders;
using WebAppShared.WebSys.DI;

namespace SwipetorApp.Services.Storage;

[Service(InterfaceToBind = typeof(IStorageBucket))]
[UsedImplicitly]
public class StorageBucket(IOptions<StorageConfig> storageConfig)
    : StorageBucketBase(storageConfig.Value), IStorageBucket
{
    public string Sprites => GetBucket("sprites");
}