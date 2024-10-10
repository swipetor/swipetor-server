using WebAppShared.WebPush;

namespace SwipetorApp.Services.WebPush;

public class WebPushTag(string name) : WebPushTagBase(name)
{
    public static WebPushTag NewMultiNotifications => new("NewMultiNotifications");
    public static WebPushTag NewPost => new("NewPost");
    public static WebPushTag NewPostComment => new("NewPostComment");
    public static WebPushTag NewMentionInPost => new("NewMentionInPost");
    public static WebPushTag NewMentionInComment => new("NewMentionInComment");
    public static WebPushTag NewPmMsg => new("NewPmMsg");
    public static WebPushTag RevealMediaByNotif => new("RevealMediaByNotif");
}