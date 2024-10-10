using System;
using SwipetorApp.Models.DbEntities;

namespace SwipetorApp.Models.DTOs;

public class SubDto
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public long StartedAt { get; set; }
    
    public long? EndedAt { get; set; }
    
    public bool IsActive { get; set; }
    
    public int PlanId { get; set; }
    public SubPlanDto Plan { get; set; }
}