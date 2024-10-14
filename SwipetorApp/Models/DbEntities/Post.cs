using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.X86;
using JetBrains.Annotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using WebLibServer.Types;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class Post : IDbEntity
{
    public int Id { get; set; }

    [MaxLength(128)]
    public string Title { get; set; }

    public int UserId { get; set; }
    [CanBeNull]
    public virtual User User { get; set; }

    public int CommentsCount { get; set; }
    
    [IndexColumn]
    public int FavCount { get; set; }
    
    [IndexColumn]
    public bool IsPublished { get; set; }

    [IndexColumn]
    public bool IsRemoved { get; set; }

    [IndexColumn]
    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    [IndexColumn, MaxLength(64)]
    public string CreatedIp { get; set; }
    
    public virtual PostNotifBatch NotifBatch { get; set; }
    
    public virtual ICollection<FavPost> Favs { get; set; }
    
    public virtual ICollection<PostHub> PostHubs { get; set; }
    public virtual ICollection<PostView> PostViews { get; set; }

    public virtual ICollection<PostMedia> Medias { get; set; }
    public virtual ICollection<Comment> Comments { get; set; }
}