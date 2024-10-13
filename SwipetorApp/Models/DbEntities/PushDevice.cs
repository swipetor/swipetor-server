using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using WebLibServer.Types;
using WebLibServer.WebPush;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class PushDevice : IDbEntity, IPushDevice
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public virtual User User { get; set; }

    [MaxLength(64)]
    public string CreatedIp { get; set; }

    [IndexColumn]
    public DateTime? LastUsedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    [Required]
    [MaxLength(1024)]
    [IndexColumn]
    public string Token { get; set; }
}