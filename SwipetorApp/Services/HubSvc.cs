using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Services.Contexts;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services;

[Service]
[UsedImplicitly]
public class HubSvc(IDbProvider dbProvider)
{
    /// <summary>
    ///     Cleans the label name, replaces spaces with dashes
    /// </summary>
    /// <param name="hubName"></param>
    /// <returns></returns>
    public string CleanHubName(string hubName)
    {
        var name = (hubName ?? "").Trim().Replace("  ", " ").Replace(" ", "-");
        return name;
        // var str = Regex.Replace(forumName, "[^A-Za-z0-9-., ]", "");
        // str = str.Replace(" ", "-").Replace("--", "-");
        // return str;
    }

    public Hub GetById(int hubId)
    {
        using var db = dbProvider.Create();
        return db.Hubs.SingleOrDefault(f => f.Id == hubId);
    }

    public List<Hub> GetOrCreateHubsByName(List<string> hubNames)
    {
        hubNames = hubNames.Select(CleanHubName).ToList();

        using var db = dbProvider.Create();
        var hubs = db.Hubs.Where(c => hubNames.Contains(c.Name)).ToList();

        var missingHubs = hubNames.Where(cn => hubs.All(c => c.Name != cn)).ToList();

        var newHubs = new List<Hub>();
        foreach (var ms in missingHubs)
        {
            var c = new Hub
            {
                Name = ms
            };
            newHubs.Add(c);
            db.Hubs.Add(c);
        }

        db.SaveChanges();

        hubs.AddRange(newHubs);
        return hubs;
    }
}