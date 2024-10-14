using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using SwipetorApp.Models.Enums;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using WebLibServer.Types;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class PostMedia : IDbEntity
{
    public int Id { get; set; }

    public int PostId { get; set; }
    public virtual Post Post { get; set; }

    public Guid? PhotoId { get; set; }
    public virtual Photo Photo { get; set; }

    public Guid? VideoId { get; set; }
    public virtual Video Video { get; set; }

    public Guid? ClippedVideoId { get; set; }
    public virtual Video ClippedVideo { get; set; }

    public Guid? PreviewPhotoId { get; set; }
    public virtual Photo PreviewPhoto { get; set; }

    public double? PreviewPhotoTime { get; set; }

    public List<List<double>> ClipTimes { get; set; }

    public bool IsFollowersOnly { get; set; }
    
    public int? SubPlanId { get; set; }
    
    [CanBeNull]
    public virtual SubPlan SubPlan { get; set; }

    [IndexColumn]
    public PostMediaType Type { get; set; }

    [MaxLength(512)]
    public string Description { get; set; }

    [MaxLength(1024 * 1000)]
    public string Article { get; set; }

    [IndexColumn]
    public bool IsInstant { get; set; }

    [IndexColumn]
    public int Ordering { get; set; }
}