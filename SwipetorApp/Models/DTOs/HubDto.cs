using JetBrains.Annotations;

namespace SwipetorApp.Models.DTOs;

public class HubDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public long LastPostAt { get; set; }

    public int Ordering { get; set; }
    public int PostCount { get; set; }

    [CanBeNull]
    public PhotoDto Photo { get; set; }
}