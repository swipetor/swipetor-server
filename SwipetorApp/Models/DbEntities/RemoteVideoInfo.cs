using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using SwipetorApp.Models.Enums;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace SwipetorApp.Models.DbEntities;

public class RemoteVideoInfo
{
    [MaxLength(128)]
    public string RefDomain { get; set; }
    
    [MaxLength(256)]
    public string RefId { get; set; }

    [MaxLength(1024)]
    public string ExtraInfo { get; set; }

    [MaxLength(64)]
    [IndexColumn]
    public RemoteVideoInfoAction Action { get; set; }

    public DateTime CreatedAt { get; set; }
}