using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SwipetorApp.Services.Referring;

namespace SwipetorApp.System.MvcFilters;

public class ProcessLinkRefAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var refValue = filterContext.HttpContext.Request.Query["rfid"];
        if (string.IsNullOrWhiteSpace(refValue)) return;
        
        var referrerSvc = filterContext.HttpContext.RequestServices.GetService<ReferrerSvc>();
        
        var refValueInt = int.Parse(refValue);
        
        // Check if "ref" parameter exists and process it
        if (!string.IsNullOrEmpty(refValue) && refValueInt > 0)
        {
            referrerSvc.ProcessReferrer(refValueInt);
        }

        base.OnActionExecuting(filterContext);
    }
}