using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.X86;
using JetBrains.Annotations;
using WebLibServer.Types;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class PmMsg : IDbEntity
{
    public long Id { get; set; }

    public long ThreadId { get; set; }
    public virtual PmThread Thread { get; set; }

    public int ThreadUserId { get; set; }
    public virtual PmThreadUser ThreadUser { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; }

    [MaxLength(65536)]
    public string Txt { get; set; }
    public DateTime CreatedAt { get; set; }
}