using JetBrains.Annotations;

namespace SwipetorApp.Models.DTOs;

[UsedImplicitly]
public class PmMsgDto
{
    public long Id { get; set; }

    public long ThreadId { get; set; }

    public int UserId { get; set; }

    public string Txt { get; set; }

    public long CreatedAt { get; set; }
}
