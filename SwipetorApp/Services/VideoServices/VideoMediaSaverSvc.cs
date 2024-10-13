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
using WebLibServer.DI;
using WebLibServer.Videos;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services.VideoServices;

[UsedImplicitly]
[Service]
public class VideoMediaSaverSvc(
    IFactory<VideoWithPreviewUploader> videoWithPreviewUploaderFactory,
    ILogger<VideoMediaSaverSvc> logger,
    IDbProvider dbProvider)
    : IDisposable
{
    private Post _post;

    private VideoWithPreviewUploader.Result _videoUploadResult;

    private Video VideoEntity { get; set; }
    private PostMedia MediaEntity { get; set; }

    /// <summary>
    ///     If provided, it is saved into db as Video.ReferenceUrl.
    /// </summary>
    public string VideoReferenceUrl { get; set; }

    public void Dispose()
    {
    }

    public async Task Save(int postId, VideoWithPreviewUploader.Result videoUploadResult, bool isInstant = false)
    {
        try
        {
            await GetPost(postId);

            _videoUploadResult = videoUploadResult;

            SaveVideoIntoDb(isInstant);
        }
        catch (Exception)
        {
            await Revert();
            throw;
        }
    }

    public async Task<PostMedia> Save(int postId, string vidFilePath, bool isInstant = false)
    {
        try
        {
            await GetPost(postId);

            await using var vidUploader = videoWithPreviewUploaderFactory.GetInstance();

            _videoUploadResult = await vidUploader.Run(vidFilePath);

            return SaveVideoIntoDb(isInstant);

            // await _mediaPreviewSaver.Save(MediaEntity, _generateAndUpload.GeneratedPreviewLocalPath);
        }
        catch (Exception)
        {
            await Revert();
            throw;
        }
    }

    private async Task GetPost(int postId)
    {
        await using var db = dbProvider.Create();
        _post = db.Posts
            .Include(p => p.User)
            .Include(p => p.Medias)
            .Where(p => p.Id == postId).Single();
    }

    private PostMedia SaveVideoIntoDb(bool isInstant = false)
    {
        logger.LogInformation("Saving uploaded video, preview and sprite into the database");
        using var db = dbProvider.Create();

        var vur = _videoUploadResult.VideoUploadResult;

        VideoEntity = new Video
        {
            Id = vur.Id,
            Ext = vur.Ext,
            Duration = vur.Duration,
            Formats = new List<VideoResolution> { vur.Format },
            Height = vur.Height,
            Width = vur.Width,
            SizeInBytes = vur.SizeInBytes,
            ReferenceUrl = VideoReferenceUrl,
            Checksum = vur.Checksum,
            Sprites = new List<Sprite>()
        };

        // foreach (var sprite in _videoUploadResult.SpriteResult)
        // {
        // 	VideoEntity.Sprites.Add(new()
        // 	{
        // 		Id = sprite.Guid,
        // 		StartTime = 0,
        // 		Interval = sprite.Interval,
        // 		ThumbnailCount = sprite.Count,
        // 		ThumbnailHeight = sprite.Height,
        // 		ThumbnailWidth = sprite.Width,
        // 	});
        // }

        var previewResult = _videoUploadResult.PreviewUploadResult;
        MediaEntity = new PostMedia
        {
            PostId = _post.Id,
            Type = PostMediaType.Video,
            VideoId = VideoEntity.Id,
            Ordering = (_post.Medias.Max(pm => (int?)pm.Ordering) ?? 0) + 1,
            PreviewPhoto = new Photo
            {
                Ext = previewResult.Ext,
                Width = previewResult.Width,
                Height = previewResult.Height,
                Sizes = new List<int>(),
                Id = previewResult.Id
            },
            PreviewPhotoTime = 0,
            Video = VideoEntity,
            IsInstant = isInstant
        };

        db.PostMedias.Add(MediaEntity);

        db.SaveChanges();
        
        return MediaEntity;
    }

    private async Task Revert()
    {
        logger.LogWarning("Reverting VideoMediaSaveFlow");

        if (MediaEntity != null && MediaEntity.Id != 0)
        {
            await using var db = dbProvider.Create();
            await db.PostMedias.Where(pm => pm.Id == MediaEntity.Id).DeleteFromQueryAsync();
        }
    }
}