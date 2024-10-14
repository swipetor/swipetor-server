using WebLibServer.Metrics;

namespace SwipetorApp.Models.Enums;

public class AppEvent : BaseAppEvent
{
    private AppEvent(string name) : base(name)
    {
    }

    // Generic user login
    public static AppEvent UserLogin => new("UserLogin");
    
    // Sending WebPush notifications, value is for per device (not per user)
    public static AppEvent NotifWebPushMultiDevice => new("NotifWebPushMultiDevice");

    // Sending notification web push to number of users
    public static AppEvent NotifWebPushUsers => new("NotifWebPushUsers");

    // Sending PM email to number of users
    public static AppEvent PmEmailUsers => new("PmEmailUsers");
    
    // Sending PM email into thread
    public static AppEvent PmMsg => new("PmMsg");

    // Sending notification email to number of users
    public static AppEvent NotifEmailToUsers => new("NotifEmailToUsers");
    public static AppEvent NotifPostCreated => new("NotifPostCreated");
}