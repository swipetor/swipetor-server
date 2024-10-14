using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using WebLibServer.Types;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class PmThread : IDbEntity
{
    public long Id { get; set; }

    public int UserCount { get; set; }

    public long? LastMsgId { get; set; }
    public virtual PmMsg LastMsg { get; set; }

    [IndexColumn]
    public DateTime LastMsgAt { get; set; }

    [IndexColumn]
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<PmMsg> Msgs { get; set; }
    public virtual ICollection<PmThreadUser> ThreadUsers { get; set; }
}
