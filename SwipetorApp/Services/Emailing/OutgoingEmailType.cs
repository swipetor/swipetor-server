using WebLibServer.Emailing;

namespace SwipetorApp.Services.Emailing;

public class OutgoingEmailType : EmailTypeBase
{
    public static readonly OutgoingEmailType LoginCode = new("LoginCode");
    public static readonly OutgoingEmailType Generic = new("Generic");
    public static readonly OutgoingEmailType Pm = new("Pm");
    public static readonly OutgoingEmailType Notif = new("Notif");

    private OutgoingEmailType(string name) : base(name)
    {
    }
}