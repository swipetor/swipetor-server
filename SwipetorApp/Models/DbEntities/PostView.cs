using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using WebAppShared.Types;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class PostView : IDbEntity
{
    public int UserId { get; set; }
    public virtual User User { get; set; }

    public int PostId { get; set; }
    public virtual Post Post { get; set; }

    public DateTime ViewedAt { get; set; }

    [MaxLength(64)]
    public string CreatedIp { get; set; }
}