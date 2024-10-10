using System;

namespace SwipetorApp.Models.DTOs;

public class SpriteDto
{
    public Guid Id { get; set; }

    public int VideoId { get; set; }

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