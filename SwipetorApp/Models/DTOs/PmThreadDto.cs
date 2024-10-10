using System.Collections.Generic;
using JetBrains.Annotations;

namespace SwipetorApp.Models.DTOs;

[UsedImplicitly]
public class PmThreadDto
{
    public long Id { get; set; }

    public int UserCount { get; set; }

    public bool IsGroupChat { get; set; }

    public int LastMsgId { get; set; }
    public virtual PmMsgDto LastMsg { get; set; }

    public long LastMsgAt { get; set; }

    public long ExpirationDate { get; set; }

    public long CreatedAt { get; set; }

    public int UnreadMsgCount { get; set; }

    public virtual List<PmThreadUserDto> ThreadUsers { get; set; }
}