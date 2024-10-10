using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.Permissions;
using WebAppShared.WebSys.DI;

namespace SwipetorApp.Services.Medias;

[Service]
[UsedImplicitly]
public class MediaSvc(IDbProvider dbProvider, UserCx userCx)
{
    /// <summary>
    ///     Duplicates a media
    /// </summary>
    public PostMedia Duplicate(int mediaId)
    {
        using var db = dbProvider.Create();
        var media = db.PostMedias
            .Include(m => m.Post).ThenInclude(p => p.Medias)
            .Single(m => m.Id == mediaId);
        
        new PostPerms().CanEdit(media.Post, userCx.Value);

        var duplicateMedia = new PostMedia
        {
            Ordering = (media.Post.Medias.MaxBy(m => m.Ordering)?.Ordering ?? 0) + 1,
            PhotoId = media.PhotoId,
            VideoId = media.VideoId,
            PostId = media.PostId,
            Description = media.Description,
            Type = media.Type,
            IsFollowersOnly = media.IsFollowersOnly,
            ClipTimes = media.ClipTimes,
            PreviewPhotoId = media.PreviewPhotoId
        };
        db.PostMedias.Add(duplicateMedia);
        db.SaveChanges();

        return duplicateMedia;
    }
}