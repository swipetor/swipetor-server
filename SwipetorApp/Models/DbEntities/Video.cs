using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using WebAppShared.Types;
using WebAppShared.Videos;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class Video : IDbEntity
{
    public Guid Id { get; set; }

    [MaxLength(8)]
    public string Ext { get; set; }

    [IndexColumn]
    public long SizeInBytes { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    // Duration in seconds
    public double Duration { get; set; }

    [IndexColumn]
    [MaxLength(256)]
    public string Checksum { get; set; }

    [MaxLength(1024*100)]
    public string Captions { get; set; }

    [MaxLength(1024)]
    public string ReferenceUrl { get; set; }

    [MaxLength(128), IndexColumn, CanBeNull]
    public string ReferenceDomain { get; set; }

    [MaxLength(128), IndexColumn, CanBeNull]
    public string ReferenceId { get; set; }

    [MaxLength(512), CanBeNull]
    public string ReferenceTitle { get; set; }

    [MaxLength(1024 * 100), CanBeNull]
    public string ReferenceDesc { get; set; }

    [MaxLength(1024 * 1000), CanBeNull]
    public string ReferenceJson { get; set; }
    
    public virtual ICollection<Sprite> Sprites { get; set; }

    [Column(TypeName = "jsonb")]
    public List<VideoResolution> Formats { get; set; }

    public DateTime CreatedAt { get; set; }

    [Required]
    [MaxLength(64)]
    public string CreatedIp { get; set; }
}