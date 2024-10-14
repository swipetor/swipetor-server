using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using WebLibServer.Types;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class Comment : IDbEntity
{
    public int Id { get; set; }

    [Required]
    [MaxLength(65536)]
    public string Txt { get; set; }

    public int PostId { get; set; }
    public virtual Post Post { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; }

    [IndexColumn]
    public int LikeCount { get; set; }

    [Required]
    [MaxLength(64)]
    public string CreatedIp { get; set; }

    public DateTime CreatedAt { get; set; }

    [MaxLength(64)]
    public string ModifiedIp { get; set; }

    public DateTime? ModifiedAt { get; set; }
}