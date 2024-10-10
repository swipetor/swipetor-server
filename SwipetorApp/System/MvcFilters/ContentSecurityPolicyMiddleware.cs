using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SwipetorApp.System.MvcFilters;

public class ContentSecurityPolicyMiddleware
{
    private readonly RequestDelegate _next;

    public ContentSecurityPolicyMiddleware()
    {
    }

    public ContentSecurityPolicyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        // context.Response.Headers.AccessControlAllowOrigin = new[] {"https://www.youtube.com"};
        // context.Response.Headers.ContentSecurityPolicy =

        return _next(context);
    }
}