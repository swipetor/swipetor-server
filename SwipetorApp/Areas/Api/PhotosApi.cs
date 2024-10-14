using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Config;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.PhotoServices;
using WebLibServer.Exceptions;
using WebLibServer.Photos;
using WebLibServer.Uploaders;

namespace SwipetorApp.Areas.Api;

[ApiController]
[Area(AreaNames.Api)]
[Route("api/photos")]
public class PhotosApi(
    IOptions<StorageConfig> storageConfig,
    UploadedPhotoResizerSvc resizerSvc,
    IStorageBucket bucket,
    IDbProvider dbProvider)
    : Controller
{
    [Route("{photoId:guid}/sizes/{size:int}")]
    public async Task<IActionResult> Index(Guid photoId, int size)
    {
        CommonPhotoUtils.AssertPhotoSizeValid(size);

        await using var db = dbProvider.Create();
        var photo = db.Photos.SingleOrDefault(p => p.Id == photoId);

        if (photo == null) throw new HttpStatusCodeException(HttpStatusCode.NotFound);

        if (!photo.Sizes.Contains(size)) await resizerSvc.Resize(photo.Id, size);

        return Redirect($"https://{storageConfig.Value.MediaHost}/{bucket.Photos}/{photo.GetFilename(size)}");
    }
}