using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Contexts;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services.Notifs;

[Service]
[UsedImplicitly]
public class NotifSvc(IDbProvider dbProvider, ILogger<NotifSvc> logger)
{
    /// <summary>
    /// TODO: Add a new notif for each mentioned user in the comment
    /// </summary>
    /// <param name="senderUserId"></param>
    /// <param name="commentId"></param>
    /// <param name="postId"></param>
    /// <param name="mentionedUserIds"></param>
    public void NewMentionInComment(int senderUserId, int commentId, int postId, List<int> mentionedUserIds)
    {
        if (mentionedUserIds.Count == 0) return;

        using var db = dbProvider.Create();
        foreach (var userId in mentionedUserIds)
            db.Notifs.Add(new Notif
            {
                ReceiverUserId = userId,
                Type = NotifType.UserMentionInComment,
                SenderUserId = senderUserId,
                RelatedPostId = postId,
                RelatedCommentId = commentId
            });
    }

    public void NewComment(int senderUserId, int commentId, int postId, int receiverUserId)
    {
        using var db = dbProvider.Create();
        db.Notifs.Add(new Notif
        {
            ReceiverUserId = receiverUserId,
            Type = NotifType.NewComment,
            SenderUserId = senderUserId,
            RelatedPostId = postId,
            RelatedCommentId = commentId
        });
    }

    public void NewReferralPremium(int receiverUserId)
    {
        using var db = dbProvider.Create();
        db.Notifs.Add(new Notif
        {
            ReceiverUserId = receiverUserId,
            Type = NotifType.ReferralPremium,
            SenderUserId = null,
            RelatedPostId = null,
            RelatedCommentId = null
        });
        db.SaveChanges();
    }

    public async Task<int> GetNotifsToCreate()
    {
        await using var db = dbProvider.Create();
        
        logger.LogInformation("Fetching a post to create notifications for it");
        
        var post = db.Posts.Include(p => p.NotifBatch)
            .Where(p => p.IsPublished && !p.NotifBatch.IsDone)
            .OrderBy(p => p.CreatedAt).FirstOrDefault();

        if (post == null)
        {
            logger.LogInformation("No post found to create notifications for");
            return 0;
        }
        
        logger.LogInformation("Fetched post {PostId}", post?.Id);

        var batch = db.UserFollows
            .Where(uf => uf.FollowedUserId == post.UserId && uf.LastNewPostNotifAt < post.CreatedAt)
            .OrderBy(uf => uf.FollowerUserId)
            .Select(uf => uf.FollowerUserId)
            .Distinct()
            .Take(100)
            .ToList();
        
        logger.LogInformation("Fetched {BatchCount} followers to create notifications for post {PostId}", batch.Count, post.Id);
        
        foreach (var followerUserId in batch)
        {
            db.Notifs.Add(new Notif
            {
                ReceiverUserId = followerUserId,
                Type = NotifType.NewPost,
                SenderUserId = post.UserId,
                RelatedPostId = post.Id
            });

            var userFollow = db.UserFollows.First(uf =>
                uf.FollowerUserId == followerUserId && uf.FollowedUserId == post.UserId);
            userFollow.LastNewPostNotifAt = DateTime.UtcNow;
        }
        
        if (batch.Count < 100)
        {
            logger.LogInformation("Marking post {PostId} as done", post.Id);
            post.NotifBatch.IsDone = true;
        }

        post.NotifBatch.ProcessedCount += batch.Count;
        await db.SaveChangesAsync();

        return batch.Count;
    }
}