using WebAppShared.WebPush;

namespace SwipetorApp.Services.WebPush;

public class WebPushPayload : WebPushPayloadBase
{
    public WebPushPayload()
    {
    }

    public WebPushPayload(WebPushPayload o) : base(o)
    {
        UserId = o.UserId;
    }

    public int UserId { get; set; }
}