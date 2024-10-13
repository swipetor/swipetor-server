using System;
using System.Collections.Concurrent;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Services.Contexts;
using WebLibServer.SharedLogic.KVStore;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services.KVStore;

[Service(InterfaceToBind = typeof(IKeyValueStore))]
[UsedImplicitly]
public class KeyValueStore(ILogger<KeyValueStoreBase> logger, IDbProvider dbProvider) : KeyValueStoreBase(logger)
{
    protected override ConcurrentDictionary<string, string> GetAllFromDb()
    {
        using var db = dbProvider.Create();
        var kvs = db.KeyValues.ToList();
        return new ConcurrentDictionary<string, string>(kvs.ToDictionary(k => k.Key, v => v.Value));
    }

    protected override void SaveToDb(KeyValueKeyBase key, string value)
    {
        using var db = dbProvider.Create();
        var kv = db.KeyValues.SingleOrDefault(k => k.Key == key);

        if (kv != null)
        {
            kv.Value = value;
            kv.ModifiedAt = DateTime.UtcNow;
        }
        else
        {
            db.KeyValues.Add(new KeyValue
            {
                Key = key,
                Value = value,
                ModifiedAt = DateTime.UtcNow
            });
        }

        db.SaveChanges();
    }
}