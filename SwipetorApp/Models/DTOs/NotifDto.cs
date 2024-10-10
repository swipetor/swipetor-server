using JetBrains.Annotations;
using SwipetorApp.Models.Enums;

namespace SwipetorApp.Models.DTOs;

[UsedImplicitly]
public class NotifDto
{
    public int Id { get; set; }

    public int ReceiverUserId { get; set; }

    public virtual UserDto ReceiverUser { get; set; }

    public int RelatedPostId { get; set; }
    [CanBeNull] public virtual PostDto RelatedPost { get; set; }

    public int? RelatedCommentId { get; set; }
    [CanBeNull] public virtual CommentDto RelatedComment { get; set; }

    public int? SenderUserId { get; set; }
    [CanBeNull] public virtual UserDto SenderUser { get; set; }

    public PhotoDto SenderUserPhoto { get; set; }

    public NotifType Type { get; set; }

    public string Data { get; set; }

    public bool IsRead { get; set; }

    public long CreatedAt { get; set; }
}