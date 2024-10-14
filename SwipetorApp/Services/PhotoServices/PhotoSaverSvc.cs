using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Services.Contexts;
using WebLibServer.Photos;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services.PhotoServices;

[Service]
[UsedImplicitly]
public class PhotoSaverSvc(
    IPhotoFileUploader photoUploader,
    PhotoDeleterSvc photoDeleterSvc,
    ILogger<PhotoSaverSvc> logger,
    IDbProvider dbProvider)
    : IDisposable
{
    private string _referenceUrl;

    public Expression<Func<Photo, bool>> DeletePreviousClause { get; set; }

    public void Dispose()
    {
        photoUploader.Dispose();
    }

    public PhotoSaverSvc SetExtension(string ext)
    {
        photoUploader.SetExtension(ext);
        return this;
    }

    public PhotoSaverSvc SetExtensionByFileName(string fileName)
    {
        photoUploader.SetExtensionByFileName(fileName);
        return this;
    }

    public PhotoSaverSvc SetMaxWidthHeight(int maxWidthHeight)
    {
        photoUploader.SetMaxWidthHeight(maxWidthHeight);
        return this;
    }

    public PhotoSaverSvc SetSource(Stream stream)
    {
        photoUploader.SetSource(stream);
        return this;
    }

    public PhotoSaverSvc SetSource(string filePath)
    {
        photoUploader.SetSource(filePath);
        return this;
    }

    /// <summary>
    ///     Sets the reference URL of the image. This is kept only for information purposes.
    /// </summary>
    /// <param name="referenceUrl"></param>
    /// <returns></returns>
    public PhotoSaverSvc SetReferenceUrl(string referenceUrl)
    {
        _referenceUrl = referenceUrl;
        return this;
    }

    public async Task<Photo> Save()
    {
        if (DeletePreviousClause != null) await DeletePrevious();

        var uploaderResult = await photoUploader.Upload();

        var photo = new Photo
        {
            Ext = uploaderResult.Ext,
            Width = uploaderResult.Width,
            Height = uploaderResult.Height,
            Sizes = new List<int>(),
            Id = uploaderResult.Id,
            ReferenceUrl = _referenceUrl
        };

        logger.LogInformation("Saving photo {Filename} to database", uploaderResult.FullFileName);

        await using var db = dbProvider.Create();
        db.Photos.Add(photo);
        await db.SaveChangesAsync();

        return photo;
    }

    private async Task DeletePrevious()
    {
        await using var db = dbProvider.Create();

        var previousPhotos = db.Photos.Where(DeletePreviousClause).ToList();
        foreach (var photo in previousPhotos)
        {
            await photoDeleterSvc.Delete(photo.Id);
            db.Remove(photo);
        }

        await db.SaveChangesAsync();
    }
}