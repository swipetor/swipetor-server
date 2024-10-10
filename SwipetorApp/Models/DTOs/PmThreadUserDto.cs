using JetBrains.Annotations;

namespace SwipetorApp.Models.DTOs;

[UsedImplicitly]
public class PmThreadUserDto
{
    public int UserId { get; set; }
    public UserDto User { get; set; }

    public long ThreadId { get; set; }

    public int LastReadMsgId { get; set; }

    public int UnreadMsgCount { get; set; }

    public long LastMsgAt { get; set; }

    public long CreatedAt { get; set; }
}