using WebAppShared.SharedLogic.Fx;

namespace SwipetorApp.Models.DTOs;

public class SubPlanDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public CPrice CPrice { get; set; }
    public int OwnerUserId { get; set; }
}
