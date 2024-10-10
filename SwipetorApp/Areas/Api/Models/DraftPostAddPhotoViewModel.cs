using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using WebAppShared.WebSys.MvcAttributes;

namespace SwipetorApp.Areas.Api.Models;

public class DraftPostAddPhotoViewModel
{
    [CanBeNull]
    public string PhotoUrl { get; set; }

    [MaxFileSize]
    [CanBeNull]
    public IFormFile File { get; set; }
}