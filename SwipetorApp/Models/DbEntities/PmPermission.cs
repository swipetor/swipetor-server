using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using WebLibServer.Types;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class PmPermission : IDbEntity
{
    public int ReceiverUserId { get; set; }
    public virtual User ReceiverUser { get; set; }

    public int SenderUserId { get; set; }
    public virtual User SenderUser { get; set; }

    [IndexColumn]
    public bool IsAllowed { get; set; }

    [IndexColumn]
    public bool IsBlocked { get; set; }

    public DateTime CreatedAt { get; set; }
    
    [MaxLength(64)]
    public string CreatedIp { get; set; }
}