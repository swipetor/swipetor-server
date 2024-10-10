using System;
using System.Collections.Generic;
using SwipetorApp.Services.Config.UIConfigs;
using WebAppShared.Photos;

namespace SwipetorApp.Models.DTOs;

public class PhotoDto : ISharedPhoto
{
    public long CreatedAt { get; set; }

    [OutputConfigToUI]
    public Guid Id { get; set; }

    [OutputConfigToUI]
    public int Height { get; set; }

    [OutputConfigToUI]
    public int Width { get; set; }

    [OutputConfigToUI]
    public string Ext { get; set; }

    [OutputConfigToUI]
    public List<int> Sizes { get; set; }
}