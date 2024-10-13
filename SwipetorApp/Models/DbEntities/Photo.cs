using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using WebLibServer.Photos;
using WebLibServer.Types;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class Photo : IDbEntity, ISharedPhoto
{
    [MaxLength(1024)]
    [CanBeNull]
    public string ReferenceUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    [Required]
    [MaxLength(64)]
    public string CreatedIp { get; set; }

    public Guid Id { get; set; }

    public int Height { get; set; }

    public int Width { get; set; }

    [MaxLength(8)]
    public string Ext { get; set; }

    public List<int> Sizes { get; set; }
}