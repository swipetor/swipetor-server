using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Services.Contexts;
using WebLibServer.Exceptions;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services.Pm;

[Service]
[UsedImplicitly]
public class PmThreadSvc(IDbProvider dbProvider, UserIdCx userIdCx)
{
    public bool CanMsg(int toUserId, int? fromUserId = null)
    {
        fromUserId ??= (int?)userIdCx;

        if (fromUserId == null) return false;

        if (GetThreadIfExists([toUserId], fromUserId) != null)
            return true;

        if (HasCommentedMeLastWeek(toUserId)) return true;

        using var db = dbProvider.Create();

        return db.UserFollows.Any(uf => uf.FollowedUserId == fromUserId && uf.FollowerUserId == toUserId);
    }

    public bool HasCommentedMeLastWeek(int userId)
    {
        if (userIdCx.IsNull()) return false;
        
        using var db = dbProvider.Create();
        var hasCommented = db.Comments
            .Include(c => c.Post)
            .Any(c => c.UserId == userId && c.Post.UserId == userIdCx &&
                      c.CreatedAt > DateTime.UtcNow.AddDays(-7));

        return hasCommented;
    }

    public PmThread GetOrCreateThread(List<int> toUserIds, int? fromUserId = null)
    {
        fromUserId ??= userIdCx.Value;
        return GetThreadIfExists(toUserIds, fromUserId) ?? CreateThread(toUserIds, fromUserId.Value);
    }

    [CanBeNull]
    public PmThread GetThreadIfExists(List<int> toUserIds, int? fromUserId = null)
    {
        fromUserId ??= (int?)userIdCx;

        if (fromUserId == null) return null;

        var userIdsWithMe = new List<int>(toUserIds) { fromUserId.Value };

        using var db = dbProvider.Create();
        var existingThread = db.PmThreads
            .Include(t => t.LastMsg)
            .Include(t => t.ThreadUsers).ThenInclude(tu => tu.User).ThenInclude(u => u.Photo)
            .Include(t => t.ThreadUsers)
            .Where(pt =>
                pt.ThreadUsers.All(tu =>
                    userIdsWithMe.Contains(tu.UserId)) && // All thread users must be in userIdsWithMe
                pt.ThreadUsers.Count() == userIdsWithMe.Count) // Number of thread users must match userIdsWithMe count
            .SingleOrDefault();

        return existingThread;
    }

    private PmThread CreateThread(List<int> toUserIds, int fromUserId)
    {
        using var db = dbProvider.Create();
        foreach (var userId in toUserIds)
        {
            if (!CanMsg(userId, fromUserId))
            {
                var user = db.Users.Find(userId);
                throw new HttpJsonError("You can only msg to followers and recent comments: " + user?.Username);
            }
        }

        var fromThreadUser = new PmThreadUser
        {
            UserId = fromUserId,
            IsInitiator = true,
            UnreadMsgCount = 0
        };

        var toThreadUsers = toUserIds.Select(userId => new PmThreadUser
        {
            UserId = userId,
            UnreadMsgCount = 0
        });

        var pmThread = new PmThread
        {
            UserCount = toUserIds.Count + 1,
            ThreadUsers = new List<PmThreadUser>(toThreadUsers) { fromThreadUser }
        };

        db.PmThreads.Add(pmThread);
        db.SaveChanges();
        return pmThread;
    }
}