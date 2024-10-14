using System;
using System.Linq;
using System.Net.Http;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Services.Config;
using SwipetorApp.Services.Contexts;
using WebLibServer.Photos;
using WebLibServer.Uploaders;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services.PhotoServices;

[Service]
[UsedImplicitly]
public class UploadedPhotoResizerSvc(
    IDbProvider dbProvider,
    IOptions<StorageConfig> storageConfig,
    IFileUploader uploader,
    ILogger<UploadedPhotoResizerSvc> logger,
    IStorageBucket bucket)
    : UploadedPhotoResizerBase(storageConfig.Value.MediaHost, uploader, logger, bucket)
{
    protected override ISharedPhoto GetPhoto(Guid photoId)
    {
        using var db = dbProvider.Create();
        return db.Photos.Single(p => p.Id == photoId);
    }

    protected override void SaveChangesToPhoto(ISharedPhoto photo)
    {
        using var db = dbProvider.Create();
        var photoObj = (Photo)photo;
        db.Photos.Attach(photoObj);
        db.Entry(photoObj).State = EntityState.Modified;
        db.SaveChanges();
    }
}