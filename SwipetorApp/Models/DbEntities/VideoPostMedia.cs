using WebLibServer.Types;

namespace SwipetorApp.Models.DbEntities;

public class VideoPostMedia : IDbEntity
{
    public int Id { get; set; }

    public int PostMediaId { get; set; }
    public virtual PostMedia PostMedia { get; set; }
}