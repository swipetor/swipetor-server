using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Filters;
using SwipetorApp.Services.Contexts;
using WebLibServer.Contexts;

namespace SwipetorApp.System.MvcFilters;

[UsedImplicitly]
public class UpdateLastOnlineActionFilter(UserCx userCx, IDbProvider dbProvider, IConnectionCx connCx) : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var user = userCx.ValueOrNull;

        if (user == null)
        {
            base.OnActionExecuting(context);
            return;
        }

        if (user.LastOnlineAt < DateTime.UtcNow.AddMinutes(-10))
        {
            using var db = dbProvider.Create();
            db.Users.Where(u => u.Id == user.Id).UpdateFromQuery(u => new
            {
                LastOnlineAt = DateTime.UtcNow, connCx.BrowserAgent,
                ModifiedIp = connCx.IpAddress,
                LastOnlineIp = connCx.IpAddress 
            });
        }

        base.OnActionExecuting(context);
    }
}