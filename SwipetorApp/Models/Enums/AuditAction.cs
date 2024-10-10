namespace SwipetorApp.Models.Enums;

public class AuditAction
{
    public string Name { get; private set; }

    private AuditAction(string name)
    {
        Name = name;
    }

    public static AuditAction Created = new("Created");
    public static AuditAction Modified = new("Modified");
    public static AuditAction Deleted = new("Deleted");
    public static AuditAction Online = new("Online");

    public override string ToString() => Name;

    public static implicit operator string(AuditAction action) => action.Name;
    public static implicit operator AuditAction(string name) => new(name);
    
    public static bool operator ==(AuditAction a, AuditAction b) => a.Name == b.Name;
    public static bool operator !=(AuditAction a, AuditAction b) => a.Name != b.Name;
    
    public override bool Equals(object obj) => obj is AuditAction action && action.Name == Name;
    public override int GetHashCode() => Name.GetHashCode();
    
}
