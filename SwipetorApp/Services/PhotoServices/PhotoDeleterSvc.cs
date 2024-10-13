using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SwipetorApp.Services.Contexts;
using WebLibServer.Exceptions;
using WebLibServer.Photos;
using WebLibServer.WebSys.DI;

namespace SwipetorApp.Services.PhotoServices;

[Service]
[UsedImplicitly]
public class PhotoDeleterSvc(PhotoAndSizesDeleterBase photoAndSizesDeleter, IDbProvider dbProvider)
{
    /// <summary>
    ///     Does not delete it from database.
    /// </summary>
    /// <param name="photoId"></param>
    /// <exception cref="HttpJsonError"></exception>
    public async Task Delete(Guid photoId)
    {
        if (photoId == Guid.Empty) throw new HttpJsonError("Photo not found");

        await using var db = dbProvider.Create();

        var photo = db.Photos.SingleOrDefault(p => p.Id == photoId);
        if (photo == null) throw new HttpJsonError("Photo not found");

        await photoAndSizesDeleter.Delete(photo);
    }
}