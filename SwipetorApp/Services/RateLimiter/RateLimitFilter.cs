using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SwipetorApp.Services.Auth;
using SwipetorApp.Services.Contexts;

namespace SwipetorApp.Services.RateLimiter;

public class RateLimitFilter<T> : ActionFilterAttribute where T : IRateLimiter
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var svc = filterContext.HttpContext.RequestServices;
        IRateLimiter rateLimiter = svc.GetService<T>();
        var userCx = svc.GetService<UserCx>();
        
        if (userCx.ValueOrNull == null || userCx.ValueOrNull.Role < UserRole.Editor)
        {
            rateLimiter.Run();
        }

        base.OnActionExecuting(filterContext);
    }
}
