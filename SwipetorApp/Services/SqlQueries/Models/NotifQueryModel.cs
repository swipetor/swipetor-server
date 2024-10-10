using JetBrains.Annotations;
using SwipetorApp.Models.DbEntities;

namespace SwipetorApp.Services.SqlQueries.Models;

public class NotifQueryModel
{
    public Notif Notif { get; set; }

    [CanBeNull]
    public Photo SenderUserPhoto { get; set; }
}