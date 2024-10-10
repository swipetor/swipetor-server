using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwipetorApp.Areas.Api.Models;
using SwipetorApp.Models.DTOs;
using SwipetorApp.Models.Enums;
using SwipetorApp.Models.Extensions;
using SwipetorApp.Services.Auth;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.Medias;
using SwipetorApp.Services.Permissions;
using SwipetorApp.Services.PhotoServices;
using SwipetorApp.Services.RateLimiter;
using SwipetorApp.Services.RateLimiter.Rules;
using SwipetorApp.Services.VideoDownload;
using SwipetorApp.Services.VideoServices;
using SwipetorApp.Services.WebPush;
using SwipetorApp.System;
using WebAppShared.DI;
using WebAppShared.Disk;
using WebAppShared.Exceptions;
using WebAppShared.Uploaders;

namespace SwipetorApp.Areas.Api;

[ApiController]
[Area(AreaNames.Api)]
[Route("api/medias")]
[Authorize]
public class MediasApi(
    UserCx userCx,
    PhotoDeleterSvc photoDeleterSvc,
    ILogger<MediasApi> logger,
    IFactory<VideoMediaSaverSvc> videoMediaSaverSvc,
    IFileDeleter fileDeleter,
    VideoDownloaderSvc videoDownloaderSvc,
    IStorageBucket bucket,
    MediaSvc mediaSvc,
    IDbProvider dbProvider,
    WebPushSvc webPushSvc)
    : Controller
{
    /// <summary>
    ///     Add a photo to an existing post
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="model"></param>
    /// <param name="isInstant"></param>
    /// <returns></returns>
    /// <exception cref="HttpStatusCodeException"></exception>
    /// TODO - Enable this endpoint
    [HttpPost("photos")]
    [RateLimitFilter<PhotoUploadRateLimiter>]
    [MinRole(UserRole.Creator)]
    public async Task<IActionResult> UploadPhoto(int postId, [FromForm] DraftPostAddPhotoViewModel model, bool isInstant = false)
    {
        throw new HttpJsonError("Photos in posts are currently disabled, will be back soon.");
        
        /*var photoUrl = model.PhotoUrl?.Trim();

        await using var db = dbProvider.Create();
        
        var post = db.Posts.Include(p => p.User).Where(p => p.Id == postId).Single();
        new PostPerms().CanEdit(post, userCx.Value);

        if (!string.IsNullOrEmpty(photoUrl))
            await photoMediaSvc.AddPhotoByUrl(postId, photoUrl);
        else if (model.File != null)
            await photoMediaSvc.UploadPhotoIntoPost(postId, model.File, isInstant);
        else
            throw new HttpJsonError("Either upload a photo or provide a photo URL.");
        
        post = db.Posts
            .Include(p => p.User)
            .Include(p => p.Medias)
            .Where(p => p.Id == postId).Single();

        return Json(mapper.Map<PostDto>(post));*/
    }

    [HttpPost("videos")]
    [RequestSizeLimit(500 * 1024 * 1024)] //500mb limit
    [RateLimitFilter<VideoUploadRateLimiter>]
    [MinRole(UserRole.Creator)]
    public async Task<IActionResult> UploadVideo(int postId, [FromForm] DraftPostAddVideoViewModel model, bool isInstant = false)
    {
        logger.LogInformation(
            "Uploading video FileName: {FN}, Name: {Name}, Length: {Length}", model.File.FileName,
            model.File.Name, model.File.Length);

        await using (var db = dbProvider.Create())
        {
            var post = db.Posts.Include(p => p.User).Where(p => p.Id == postId).Single();
            new PostPerms().CanEdit(post, userCx.Value);
        }

        var vidFilePath = Path.GetTempFileName();

        // Read the stream until the end.
        await using (var vidFile =
                     new FileStream(vidFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, 4096))
        {
            await model.File.CopyToAsync(vidFile);
        }

        using var videoFlow = videoMediaSaverSvc.GetInstance();
        await videoFlow.Save(postId, vidFilePath, isInstant);

        return Ok();
    }
    
    [HttpPost("videos/from-url")]
    [RateLimitFilter<VideoUploadRateLimiter>]
    [MinRole(UserRole.Creator)]
    public async Task<IActionResult> ImportFromUrl(DraftPostAddVideoFromUrlViewModel model)
    {
        logger.LogInformation(
            "Importing video from url: {YU} into post {PostId}", model.Url, model.PostId);
        
        await using (var db = dbProvider.Create())
        {
            var post = db.Posts.Include(p => p.User).Where(p => p.Id == model.PostId).Single();
            new PostPerms().CanEdit(post, userCx.Value);
        }

        using var tempPath = new ScopedTempPath();
        var localFilePath = await videoDownloaderSvc.Download(model.Url, tempPath);

        using var videoFlow = videoMediaSaverSvc.GetInstance();
        videoFlow.VideoReferenceUrl = model.Url;
        await videoFlow.Save(model.PostId, localFilePath);

        return Ok();
    }

    /// <summary>
    ///     Delete media
    /// </summary>
    /// <param name="mediaId"></param>
    /// <returns></returns>
    /// <exception cref="HttpStatusCodeException"></exception>
    [HttpDelete("{mediaId}")]
    [MinRole(UserRole.Creator)]
    public async Task<IActionResult> Delete(int mediaId)
    {
        await using var db = dbProvider.Create();
        var media = db.PostMedias
            .Include(m => m.Post)
            .Include(m => m.Video).ThenInclude(video => video.Sprites)
            .Single(m => m.Id == mediaId);
        
        new PostPerms().CanDelete(media.Post, userCx.Value);
        
        logger.LogInformation("Deleting media {MediaId}", mediaId);

        db.PostMedias.Remove(media);
        await db.SaveChangesAsync();

        if (media.Type == PostMediaType.Photo && media.PhotoId != null &&
            db.Photos.Count(p => p.Id == media.PhotoId) == 1)
            await photoDeleterSvc.Delete(media.PhotoId.Value);

        // Only delete the video file if no other media is referencing it.
        if (media.Type == PostMediaType.Video && media.VideoId != null &&
            db.PostMedias.Count(m => m.VideoId == media.VideoId) <= 1)
        {
            foreach (var f in media.Video.GetFileNames()) await fileDeleter.Delete(bucket.Videos, f);

            foreach (var sprite in media.Video.Sprites) await fileDeleter.Delete(bucket.Sprites, sprite.Id + ".webp");
        }

        return Ok();
    }

    [HttpPost("{mediaId}/duplicate")]
    [MinRole(UserRole.Creator)]
    public IActionResult Duplicate(int mediaId)
    {
        mediaSvc.Duplicate(mediaId);
        return Ok();
    }

    [HttpGet("{mediaId}/move")]
    [MinRole(UserRole.Creator)]
    public IActionResult Move(int mediaId, string direction)
    {
        using var db = dbProvider.Create();
        var media = db.PostMedias
            .Include(m => m.Post).ThenInclude(p => p.Medias)
            .Single(m => m.Id == mediaId);
        
        new PostPerms().CanEdit(media.Post, userCx.Value);

        var orderedMedias = media.Post.Medias.OrderBy(m => m.Ordering);

        var swapWith = direction == "up"
            ? orderedMedias.LastOrDefault(m => m.Ordering < media.Ordering && m.Id != media.Id)
            : orderedMedias.FirstOrDefault(m => m.Ordering > media.Ordering && m.Id != media.Id);

        if (swapWith == null) return Ok();

        (media.Ordering, swapWith.Ordering) = (swapWith.Ordering, media.Ordering);
        db.SaveChanges();

        return Ok();
    }
    
    [HttpPost("{mediaId:int}/notif-reveal")]
    [Authorize]
    public async Task<IActionResult> NotifReveal(int mediaId, [FromBody] MediaNotifRevealVM model)
    {
        var db = dbProvider.Create();
        var pushDevice = webPushSvc.SaveGetPushDevice(model.Token);
        var media = db.PostMedias.Include(m => m.Post).Single(m => m.Id == mediaId);
        
        if (pushDevice == null || media == null) return NotFound();

        var payload = new WebPushPayload
        {
            UserId = userCx.Value.Id,
            Title = $"Reveal Exclusive media in post {media.Post.Title}",
            Body = "Click to reveal the media",
            Url = $"/p/{media.Post.Id}",
            PushDevice = pushDevice,
            Tag = WebPushTag.RevealMediaByNotif,
            Icon = "/public/images/hub/hub-dot-256.png",
            Data = new Dictionary<string, string>(){ { "mediaId", mediaId.ToString() } }
        };

        var payloads = new List<WebPushPayload> { payload };
        await webPushSvc.PushToDevices(payloads);
        
        return Ok();
    }
}