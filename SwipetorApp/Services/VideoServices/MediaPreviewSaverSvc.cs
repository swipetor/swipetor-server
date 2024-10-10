using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.PhotoServices;
using SwipetorApp.System.Extensions;
using WebAppShared.DI;
using WebAppShared.Uploaders;
using WebAppShared.Videos;
using WebAppShared.WebSys.DI;

namespace SwipetorApp.Services.VideoServices;

[Service]
[UsedImplicitly]
public class MediaPreviewSaverSvc(
    ILogger<MediaPreviewSaverSvc> logger,
    IFactory<PhotoSaverSvc> photoSaverFactory,
    PhotoDeleterSvc photoDeleterSvc,
    VideoPreviewGenerator previewGenerator,
    IDbProvider dbProvider,
    IStorageBucket storageBucket,
    LocalVideoPathProvider localVideoPathProvider)
    : IDisposable
{
    private LocalVideoPath _generatorLocalVideoPath; // Should be disposed of
    private string _givenLocalVideoPath; //Should not be disposed of

    private PostMedia _media;

    public void Dispose()
    {
        previewGenerator.Dispose();
        _generatorLocalVideoPath?.Dispose();
    }

    /// <summary>
    ///     Generates video preview (if needed), deletes if there is existing one, uploads and saves into db
    /// </summary>
    /// <param name="media">Media object to save new preview photo information into</param>
    /// <param name="existingPreviewPath">If not given, it will be generated.</param>
    /// <param name="givenVideoPath">Either clipped video or full video's local path, if available, so we don't download it</param>
    /// <returns></returns>
    public async Task Save(PostMedia media, string existingPreviewPath, string givenVideoPath = null)
    {
        _media = media ?? throw new Exception("Media is null");
        _givenLocalVideoPath = givenVideoPath;

        PreviewPhotoPath = existingPreviewPath ?? await previewGenerator.Generate(await GetVideoPath(), 0);

        await Upload();
        AssociateWithMediaEntity();
    }

    private async Task Upload()
    {
        await DeleteExistingPreviewPhotoIfExists();

        using var photoSaver = photoSaverFactory.GetInstance();
        PreviewPhotoEntity = await photoSaver.SetSource(PreviewPhotoPath)
            .SetMaxWidthHeight(1200)
            .Save();
    }

    private void AssociateWithMediaEntity()
    {
        using var db = dbProvider.Create();
        db.PostMedias.Where(pm => pm.Id == _media.Id).UpdateFromQuery(pm => new
        {
            PreviewPhotoId = PreviewPhotoEntity.Id,
            PreviewPhotoTime = 0.00
        });
        db.SaveChanges();

        _media.PreviewPhotoId = PreviewPhotoEntity.Id;
        _media.PreviewPhotoTime = 0;
    }

    private async Task DeleteExistingPreviewPhotoIfExists()
    {
        if (_media?.PreviewPhotoId == null) return;


        await using var db = dbProvider.Create();
        if (db.PostMedias.Count(m => m.PreviewPhotoId == _media.PreviewPhotoId) == 1)
        {
            logger.LogInformation("Deleting old video media preview photo {PhotoId}", _media.PreviewPhotoId);
            await photoDeleterSvc.Delete(_media.PreviewPhotoId.Value);
        }
    }

    /// <summary>
    ///     Get either the given local video path if exists or generate a local video path.
    /// </summary>
    /// <returns></returns>
    private async Task<string> GetVideoPath()
    {
        if (_givenLocalVideoPath != null) return _givenLocalVideoPath;

        if (_generatorLocalVideoPath != null) return await _generatorLocalVideoPath.GetLocalPath();

        _generatorLocalVideoPath ??=
            localVideoPathProvider.FromUrl((_media.ClippedVideo ?? _media.Video).GetHttpUrl(storageBucket));

        return await _generatorLocalVideoPath.GetLocalPath();
    }


    #region Getters Setters

    public string PreviewPhotoPath { get; private set; }

    public Photo PreviewPhotoEntity { get; private set; }

    #endregion
}