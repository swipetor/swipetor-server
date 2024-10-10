using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using WebAppShared.Types;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class PmThreadUser : IDbEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; }

    public long ThreadId { get; set; }
    public virtual PmThread Thread { get; set; }

    public long? FirstUnreadMsgId { get; set; }
    public virtual PmMsg FirstUnreadMsg { get; set; }

    public long? LastReadMsgId { get; set; }
    public virtual PmMsg LastReadMsg { get; set; }

    [IndexColumn]
    public int UnreadMsgCount { get; set; }

    /// <summary>
    ///     This is the user who initiated the thread last time.
    /// </summary>
    [IndexColumn]
    public bool IsInitiator { get; set; }

    [IndexColumn]
    public DateTime? EmailSentAt { get; set; }

    [IndexColumn]
    public DateTime LastMsgAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<PmMsg> Msgs { get; set; }
}