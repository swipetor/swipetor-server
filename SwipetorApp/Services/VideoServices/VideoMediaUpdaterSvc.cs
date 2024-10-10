using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SwipetorApp.Areas.Api.Models;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Services.Contexts;
using SwipetorApp.System.Extensions;
using SwipetorApp.Utils;
using WebAppShared.DI;
using WebAppShared.Disk;
using WebAppShared.Uploaders;
using WebAppShared.Videos;
using WebAppShared.WebSys.DI;

namespace SwipetorApp.Services.VideoServices;

[Service]
[UsedImplicitly]
public class VideoMediaUpdaterSvc(
    IDbProvider dbProvider,
    LocalVideoPathProvider localVideoPathProvider,
    IStorageBucket storageBucket,
    IFactory<VideoWithPreviewUploader> videoWithPreviewUploaderFactory,
    IFileDeleter fileDeleter,
    IFactory<MediaPreviewSaverSvc> mediaPreviewSaverFactory) : IDisposable
{
    private readonly ScopedTempPath _scopedTempPath = new();
    private string _clippedVideoPath;
    private PostMedia _existingMedia;
    private LocalVideoPath _localVideoPath;

    public void Dispose()
    {
        _localVideoPath?.Dispose();
        _scopedTempPath?.Dispose();

        if (_clippedVideoPath != null && File.Exists(_clippedVideoPath))
            File.Delete(_clippedVideoPath);
    }

    public async Task Update(PostSaveModel.PostMediaItemModel itemModel)
    {
        await using var db = dbProvider.Create();

        _existingMedia = db.PostMedias.Include(postMedia => postMedia.Video).Single(m => m.Id == itemModel.Id);

        _localVideoPath = localVideoPathProvider.FromUrl(_existingMedia.Video.GetHttpUrl(storageBucket));

        //Generate a clip and preview if clip times are given or updated
        if (!_existingMedia.ClipTimes.IsEqualList(itemModel.ClipTimes))
        {
            await ProcessClipFile(itemModel.ClipTimes, db);

            using var previewSaver = mediaPreviewSaverFactory.GetInstance();
            await previewSaver.Save(_existingMedia, null, _clippedVideoPath ?? await _localVideoPath.GetLocalPath());
        }

        _existingMedia.ClipTimes = itemModel.ClipTimes;
        _existingMedia.IsFollowersOnly = itemModel.IsFollowersOnly;
        _existingMedia.Description = itemModel.Description;
        _existingMedia.SubPlanId = itemModel.SubPlanId;

        await db.SaveChangesAsync();
    }

    private async Task ProcessClipFile(List<List<double>> clipTimes, DbCx db)
    {
        // Delete if an old clip exists
        if (_existingMedia.ClippedVideo != null)
            await fileDeleter.Delete(storageBucket.Videos, _existingMedia.ClippedVideo.GetHttpUrl(storageBucket));

        // Don't continue if we are deleting the clip times
        if (clipTimes == null || clipTimes.Count == 0)
        {
            _existingMedia.ClippedVideoId = null;
            return;
        }

        _clippedVideoPath =
            await new VideoClipGenerator(_scopedTempPath).GenerateClipsAndMerge(await _localVideoPath.GetLocalPath(),
                clipTimes);

        var clippedVideoResult =
            await videoWithPreviewUploaderFactory.GetInstance().Run(_clippedVideoPath);
        var clipUploadResult = clippedVideoResult.VideoUploadResult;

        db.Videos.Add(new Video
        {
            Id = clipUploadResult.Id,
            Ext = clipUploadResult.Ext,
            Width = clipUploadResult.Width,
            Height = clipUploadResult.Height,
            Duration = clipUploadResult.Duration,
            Formats = [clipUploadResult.Format],
            Checksum = clipUploadResult.Checksum,
            SizeInBytes = clipUploadResult.SizeInBytes
        });

        _existingMedia.ClippedVideoId = clipUploadResult.Id;
    }
}