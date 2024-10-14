using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using WebLibServer.Types;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class Hub : IDbEntity
{
    public int Id { get; set; }

    [Required]
    [MaxLength(32)]
    public string Name { get; set; }
    

    [IndexColumn]
    public DateTime? LastPostAt { get; set; }

    [IndexColumn]
    public int Ordering { get; set; }

    [IndexColumn]
    public int PostCount { get; set; }

    public virtual Guid? PhotoId { get; set; }
    public virtual Photo Photo { get; set; }
    public virtual ICollection<PostHub> PostHubs { get; set; }
}