using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SwipetorApp.Models.Enums;

namespace SwipetorApp.Models.DTOs;

[UsedImplicitly]
public class PostMediaDto
{
    public int Id { get; set; }

    public int PostId { get; set; }

    public PhotoDto Photo { get; set; }

    public VideoDto Video { get; set; }

    public VideoDto ClippedVideo { get; set; }

    public Guid? PreviewPhotoId { get; set; }
    public virtual PhotoDto PreviewPhoto { get; set; }

    public List<List<double>> ClipTimes { get; set; }

    public bool IsFollowersOnly { get; set; }
    
    public int? SubPlanId { get; set; }
    public SubPlanDto SubPlan { get; set; }

    public PostMediaType Type { get; set; }

    public string Description { get; set; }

    public string Article { get; set; }
    
    public bool IsInstant { get; set; }

    public int Ordering { get; set; }
}