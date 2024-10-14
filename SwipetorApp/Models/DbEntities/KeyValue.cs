using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using WebLibServer.Types;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class KeyValue : IDbEntity
{
    [Key]
    [MaxLength(64)]
    public string Key { get; set; }

    [MaxLength(4096)]
    public string Value { get; set; }

    [MaxLength(64)]
    public string ModifiedIp { get; set; }

    public DateTime? ModifiedAt { get; set; }
}