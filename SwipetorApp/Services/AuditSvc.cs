using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Contexts;
using WebAppShared.Types;
using WebAppShared.WebSys.DI;

namespace SwipetorApp.Services;

[Service]
public class AuditSvc(UserIdCx userIdCx, IDbProvider dbProvider)
{
    private AuditLog Enter<T>(T entity, AuditAction action, string logText = null) where T : class, IDbEntity
    {
        var log = new AuditLog
        {
            EntityName = typeof(T).Name,
            EntityId = entity.GetType().GetProperty("Id")?.GetValue(entity)?.ToString(),
            Action = action,
            UserId = userIdCx.Value,
            Log = logText
        };
        
        using DbCx db = dbProvider.Create();
        
        db.AuditLogs.Add(log);
        db.SaveChanges();

        return log;
    }
}