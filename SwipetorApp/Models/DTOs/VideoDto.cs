using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SwipetorApp.Models.DbEntities;
using WebLibServer.Videos;

namespace SwipetorApp.Models.DTOs;

[UsedImplicitly]
public class VideoDto
{
    public Guid Id { get; set; } = Guid.Empty;

    public string Ext { get; set; }

    public long SizeInBytes { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public double Duration { get; set; }

    public string Captions { get; set; }

    public virtual List<Sprite> Sprites { get; set; }

    public List<VideoResolution> Formats { get; set; }

    public long CreatedAt { get; set; }
}