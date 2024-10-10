using System.Linq;
using Microsoft.EntityFrameworkCore;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Services.SqlQueries.Models;

namespace SwipetorApp.Services.SqlQueries;

public static class SelectAsExtensions
{
    public static IQueryable<HubQueryModel> SelectForUser(this IQueryable<Hub> query)
    {
        return query.OrderBy(c => c.Ordering).Select(c =>
            new HubQueryModel
            {
                Hub = c,
                Photo = c.Photo
            });
    }

    public static IQueryable<PostQueryModel> SelectForUser(this IQueryable<Post> query, int? userId)
    {
        return query
            .Include(p => p.Medias).ThenInclude(m => m.PreviewPhoto)
            .Include(p => p.PostHubs).ThenInclude(c => c.Hub).ThenInclude(c => c.Photo)
            .Select(p =>
                new PostQueryModel
                {
                    Post = p,
                    User = p.User,
                    UserPhoto = p.User.Photo,
                    UserFollows = p.User.Followers.Any(f => f.FollowerUserId == userId),
                    UserFav = p.Favs.Any(f => f.UserId == userId),
                    Medias = p.Medias.AsQueryable()
                        .Include(p => p.Photo)
                        .Include(p => p.Video).ThenInclude(v => v.Sprites)
                        .Include(m => m.PreviewPhoto)
                        .Include(m => m.SubPlan)
                        .OrderBy(m => m.Ordering)
                        .ToList()
                });
    }

    public static IQueryable<CommentQueryModel> SelectForUser(this IQueryable<Comment> query, int? userId)
    {
        return query.Include(c => c.User)
            .Select(c =>
                new CommentQueryModel
                {
                    Comment = c,
                    User = c.User,
                    UserPhoto = c.User.Photo
                });
    }

    public static IQueryable<NotifQueryModel> SelectForUser(this IQueryable<Notif> query)
    {
        return query.Include(n => n.ReceiverUser)
            .Include(n => n.RelatedPost)
            .Select(n => new NotifQueryModel
            {
                Notif = n,
                SenderUserPhoto = n.SenderUser.Photo
            });
    }
    
    public static IQueryable<PmThreadQueryModel> SelectForUser(this IQueryable<PmThread> query)
    {
        return query.Include(t => t.LastMsg).Include(t => t.ThreadUsers).ThenInclude(tu => tu.User)
            .ThenInclude(u => u.Photo)
            .Select(t => new PmThreadQueryModel
            {
                PmThread = t,
                PmThreadUserQueryModel = t.ThreadUsers.AsQueryable().Select(tu => new PmThreadUserQueryModel
                {
                    ThreadUser = tu,
                    UserQueryModel = new UserQueryModel
                    {
                        User = tu.User,
                        Photo = tu.User.Photo
                    }
                }).ToList()
            });
    }
    
    public static IQueryable<UserQueryModel> SelectForUser(this IQueryable<User> query)
    {
        return query.Include(u => u.Photo)
            .Select(u => new UserQueryModel
            {
                User = u,
                Photo = u.Photo,
                UserFollows = u.Followers.Any(f => f.FollowerUserId == u.Id)
            });
    }
}