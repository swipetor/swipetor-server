using System.Linq;
using JetBrains.Annotations;
using SwipetorApp.Services.Contexts;
using WebAppShared.WebSys.DI;

namespace SwipetorApp.Services.Users;

[Service]
[UsedImplicitly]
public class UserFollowSvc(IDbProvider dbProvider, UserIdCx userIdCx)
{
    // write me a function to check if userId1 follows userId2
    public bool IsFollowing(int followerId, int? followedId = null)
    {
        followedId ??= userIdCx.Value;
        
        using var db = dbProvider.Create();
        return db.UserFollows.Any(uf => uf.FollowerUserId == followerId && uf.FollowedUserId == followedId);
    }

}