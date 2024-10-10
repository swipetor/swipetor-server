using WebAppShared.Types;

namespace SwipetorApp.Models.Enums;

public class RemoteVideoInfoAction : EnumClass<string>
{
    public static readonly RemoteVideoInfoAction Used = new("used");
    public static readonly RemoteVideoInfoAction Skip = new("skip");
    
    public RemoteVideoInfoAction()
    {
    }
    
    public RemoteVideoInfoAction(string value) : base(value)
    {
    }
}