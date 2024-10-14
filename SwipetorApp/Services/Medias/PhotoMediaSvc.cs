using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.PhotoServices;
using WebLibServer.DI;
using WebLibServer.Exceptions;
using WebLibServer.Http;
using WebLibServer.Photos;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services.Medias;

[Service]
[UsedImplicitly]
public class PhotoMediaSvc(
    UserCx cu,
    IFactory<PhotoSaverSvc> photoSaverFactory,
    IDbProvider dbProvider,
    IHttpClientFactory httpClientFactory)
{
    public async Task<PostMedia> UploadPhotoIntoPost(int postId, IFormFile file, bool isInstant = false)
    {
        using var uploader = photoSaverFactory.GetInstance()
            .SetSource(file.OpenReadStream())
            .SetMaxWidthHeight(5120);

        var photo = await uploader.Save();

        await using var db = dbProvider.Create();
        var post = db.Posts.Include(p => p.User).Include(p => p.Medias)
            .Where(p => p.Id == postId).Single();

        var postMedia = SavePostMedia(new PostMedia
        {
            PostId = post.Id,
            PhotoId = photo.Id,
            Type = PostMediaType.Photo,
            IsInstant = true
        });
        
        return postMedia;
    }

    public async Task<PostMedia> AddPhotoByUrl(int postId, string photoUrl)
    {
        if (string.IsNullOrEmpty(photoUrl))
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "photoUrl is required");

        var filename = Path.GetFileName(new Uri(photoUrl).AbsolutePath);
        var ext = Path.GetExtension(filename);

        if (!CommonPhotoUtils.IsExtensionPhotoFile(ext)) throw new HttpJsonError("Not an acceptable photo file.");

        await using var db = dbProvider.Create();
        var post = db.Posts.Include(p => p.User).Include(p => p.Medias)
            .Where(p => p.Id == postId).Single();

        photoUrl = photoUrl.Trim();

        if (post.UserId != cu.Value.Id) throw new HttpStatusCodeException(HttpStatusCode.Unauthorized);

        await using var memoryStream = new MemoryStream();
        var result = await httpClientFactory.ChromeClient().GetStreamAsync(photoUrl);
        result.CopyTo(memoryStream);
        memoryStream.Position = 0;

        memoryStream.Flush();

        using var uploader = photoSaverFactory.GetInstance()
            .SetSource(memoryStream)
            .SetMaxWidthHeight(5120)
            .SetReferenceUrl(photoUrl);

        var photo = await uploader.Save();

        var postMedia = SavePostMedia(new PostMedia
        {
            PostId = post.Id,
            PhotoId = photo.Id,
            Type = PostMediaType.Photo
        });

        return postMedia;
    }

    /// <summary>
    ///     Saves a given PostMedia with the correct ordering
    /// </summary>
    /// <returns></returns>
    public PostMedia SavePostMedia(PostMedia postMedia)
    {
        using var db = dbProvider.Create();
        postMedia.Ordering =
            (db.PostMedias.Where(pm => pm.PostId == postMedia.PostId).Max(pm => (int?)pm.Ordering) ?? 0) + 1;

        db.PostMedias.Add(postMedia);
        db.SaveChanges();

        return postMedia;
    }
}