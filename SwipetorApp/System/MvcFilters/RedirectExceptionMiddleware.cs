using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SwipetorApp.System.Extensions;

namespace SwipetorApp.System.MvcFilters;

public class RedirectExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (RedirectException ex)
        {
            context.Response.Redirect(ex.RedirectUrl, ex.IsPermanent);
        }
    }
}