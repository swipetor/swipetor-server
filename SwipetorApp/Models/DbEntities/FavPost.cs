using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using WebAppShared.Types;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class FavPost : IDbEntity
{
    public int Id { get; set; }

    [IndexColumn]
    public int PostId { get; set; }
    public virtual Post Post { get; set; }

    [IndexColumn]
    public int UserId { get; set; }
    public virtual User User { get; set; }

    public DateTime CreatedAt { get; set; }
    
    [MaxLength(64)]
    public string CreatedIp { get; set; }
}