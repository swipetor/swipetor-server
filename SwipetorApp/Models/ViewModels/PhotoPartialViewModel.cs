using AutoMapper;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.DTOs;
using WebAppShared.WebSys.DI;

namespace SwipetorApp.Models.ViewModels;

public class PhotoPartialViewModel
{
    public PhotoPartialViewModel(Photo photo, int? size = null)
    {
        Photo = photo != null ? Mapper.Map<PhotoDto>(photo) : null;
        Size = size;
    }

    public PhotoPartialViewModel(PhotoDto photo, int? size = null)
    {
        Photo = photo;
        Size = size;
    }

    public PhotoDto Photo { get; set; }
    public int? Size { get; set; }

    public IMapper Mapper => ServiceLocator.Get<IMapper>();
}