using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Services.Contexts;
using WebAppShared.WebSys.DI;

namespace SwipetorApp.Services;

[Service]
[UsedImplicitly]
public class PostSvc(IDbProvider dbProvider)
{
    public void UpdateHubs(int postId, List<int> newHubIds, List<int> oldHubIds)
    {
        var intersectCids = oldHubIds.Intersect(newHubIds).ToList();

        var cidsToAdd = newHubIds.Except(intersectCids);
        var cidToRemove = oldHubIds.Except(newHubIds).ToList();

        using var db = dbProvider.Create();
        db.PostHubs.Where(pc => pc.PostId == postId && cidToRemove.Contains(pc.HubId)).DeleteFromQuery();
        db.PostHubs.AddRange(cidsToAdd.Select(cid => new PostHub { HubId = cid, PostId = postId }));
        db.SaveChanges();
    }
    
    public void StartNotifBatchIfNotExists(int postId)
    {
        using var db = dbProvider.Create();
        if (db.PostNotifBatches.Any(pnb => pnb.PostId == postId))
            return;
        
        db.PostNotifBatches.Add(new PostNotifBatch { PostId = postId });
        db.SaveChanges();
    }
}