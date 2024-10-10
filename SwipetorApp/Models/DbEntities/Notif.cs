using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using SwipetorApp.Models.Enums;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using WebAppShared.Types;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class Notif : IDbEntity
{
    public int Id { get; set; }

    [IndexColumn]
    public int ReceiverUserId { get; set; }

    public virtual User ReceiverUser { get; set; }

    [IndexColumn]
    public int? RelatedPostId { get; set; }

    [CanBeNull]
    public virtual Post RelatedPost { get; set; }

    [IndexColumn]
    public int? RelatedCommentId { get; set; }

    [CanBeNull]
    public virtual Comment RelatedComment { get; set; }

    [IndexColumn]
    public int? SenderUserId { get; set; }

    [CanBeNull]
    public virtual User SenderUser { get; set; }

    [IndexColumn]
    public NotifType Type { get; set; }

    [MaxLength(4096)]
    [CanBeNull]
    public string Data { get; set; }

    /// <summary>
    ///     A notification is only read when it's clicked on or clicked on the mark read button.
    /// </summary>
    [IndexColumn]
    public bool IsRead { get; set; }

    /// <summary>
    ///     A Viewed notification does not increase unread notifications count on top bar.
    ///     It means the notification was shown to the user on the notifications page
    /// </summary>
    [IndexColumn]
    public bool IsViewed { get; set; }

    [IndexColumn]
    public DateTime? PushNotifSentAt { get; set; }

    [IndexColumn]
    public DateTime CreatedAt { get; set; }
}