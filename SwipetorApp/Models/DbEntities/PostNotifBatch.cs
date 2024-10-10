using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using JetBrains.Annotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace SwipetorApp.Models.DbEntities;

public class PostNotifBatch
{
    [Key]
    public int PostId { get; set; }
    public virtual Post Post { get; set; }

    public int ProcessedCount { get; set; }

    [IndexColumn]
    public bool IsDone { get; set; }

    [IndexColumn]
    public DateTime CreatedAt { get; set; }
}