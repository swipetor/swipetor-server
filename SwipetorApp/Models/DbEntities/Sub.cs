using System;
using System.ComponentModel.DataAnnotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace SwipetorApp.Models.DbEntities;

public class Sub
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public virtual User User { get; set; }
    
    public DateTime StartedAt { get; set; }
    
    public DateTime? EndedAt { get; set; }
    
    public bool IsActive { get; set; }
    public int PlanId { get; set; }
    public virtual SubPlan Plan { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public DateTime? ModifiedAt { get; set; }

    [MaxLength(2048)]
    public string BrowserAgent { get; set; }
    
    [IndexColumn, MaxLength(64)]
    public string CreatedIp { get; set; }
}