using JetBrains.Annotations;

namespace SwipetorApp.Models.DTOs;

[UsedImplicitly]
public class CommentDto
{
    public int Id { get; set; }

    public string Txt { get; set; }

    public int UserId { get; set; }
    public virtual UserDto User { get; set; }

    public int PostId { get; set; }

    public long CreatedAt { get; set; }
}