using JetBrains.Annotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using WebAppShared.Types;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class PostHub : IDbEntity
{
    public int PostId { get; set; }
    public virtual Post Post { get; set; }

    [IndexColumn]
    public int HubId { get; set; }

    public virtual Hub Hub { get; set; }
}