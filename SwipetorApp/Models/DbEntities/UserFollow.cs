using System;
using JetBrains.Annotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using WebLibServer.Types;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class UserFollow : IDbEntity
{
    public int FollowerUserId { get; set; }
    public virtual User FollowerUser { get; set; }
    
    public int FollowedUserId { get; set; }
    public virtual User FollowedUser { get; set; }

    [IndexColumn]
    public DateTime LastNewPostNotifAt { get; set; }
}
