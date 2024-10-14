using System;
using JetBrains.Annotations;
using WebLibServer.Types;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class Sprite : IDbEntity
{
    public Guid Id { get; set; }

    public Guid VideoId { get; set; }
    public virtual Video Video { get; set; }

    /// <summary>
    ///     Start time in seconds
    /// </summary>
    public double StartTime { get; set; }

    /// <summary>
    ///     Interval in seconds
    /// </summary>
    public double Interval { get; set; }

    public int ThumbnailCount { get; set; }

    public int ThumbnailWidth { get; set; }

    public int ThumbnailHeight { get; set; }
}