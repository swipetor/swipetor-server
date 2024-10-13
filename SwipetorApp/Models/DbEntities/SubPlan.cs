using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using WebLibServer.SharedLogic.Fx;

namespace SwipetorApp.Models.DbEntities;

public class SubPlan
{
    public int Id { get; set; }
    
    [MaxLength(30)]
    public string Name { get; set; }
    
    [MaxLength(255)]
    public string Description { get; set; }

    public Currency Currency { get; set; }
    
    [Index]
    public decimal? Price { get; set; }
    
    [NotMapped]
    public CPrice CPrice => new(Currency, Price);
    
    public int OwnerUserId { get; set; }
    public virtual User OwnerUser { get; set; }
    
    public virtual ICollection<Sub> Subscriptions { get; set; }
}
