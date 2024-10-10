using System.Collections.Generic;
using System.Linq;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.Enums;

namespace SwipetorApp.Services.WebPush.Notifs;

public class NotifWebPushFormatter(int userId, List<Notif> notifs)
{
    private readonly WebPushPayload _payload = new()
    {
        UserId = userId,
        Url = "/notifs",
        Icon = "/public/images/hub/hub-dot-256.png"
    };

    private Notif _firstNotif;

    public WebPushPayload Format()
    {
        if (!notifs.Any()) return null;

        _firstNotif = notifs.First();

        if (notifs.Count > 1)
            MultiNotificationFormatter();
        else if (_firstNotif.Type == NotifType.UserMentionInComment)
            UserMentionInCommentFormatter();
        else if (_firstNotif.Type == NotifType.NewPost)
            NewPostFormatter();
        else
            MultiNotificationFormatter();

        return _payload;
    }

    private void MultiNotificationFormatter()
    {
        _payload.Title = "Swipetor activity";
        _payload.Body = "You have multiple notifications.";
        _payload.Tag = WebPushTag.NewMultiNotifications;
    }

    private void UserMentionInCommentFormatter()
    {
        _payload.Title = "Swipetor new mention";
        _payload.Body = "You are mentioned in a comment";
        _payload.Tag = WebPushTag.NewMentionInComment;
    }
    
    private void NewPostFormatter()
    {
        _payload.Title = "There is a new post";
        _payload.Body = "From a user you follow";
        _payload.Tag = WebPushTag.NewPost;
    }
    
}