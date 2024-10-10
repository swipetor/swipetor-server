using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Contexts;

namespace SwipetorApp.Services.Auditing;

public class UserOnlineAuditSvc(IDbProvider dbProvider, UserIdCx userIdCx)
{
    public void RecordUserOnline()
    {
        using var db = dbProvider.Create();

        var log = new AuditLog
        {
            Action = AuditAction.Online,
            UserId = userIdCx.Value
        };

        db.AuditLogs.Add(log);
        db.SaveChanges();
    }
}