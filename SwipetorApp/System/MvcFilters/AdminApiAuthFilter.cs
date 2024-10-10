using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using SwipetorApp.Services.Config;

namespace SwipetorApp.System.MvcFilters;

public class AdminApiAuthFilter(IOptions<HostMasterConfig> apiConfig) : Attribute, IAuthorizationFilter
{
    // public AdminApiAuthFilter()
    // {
    // 	throw new Exception("Cannot use this constructor without dependencies.");
    // }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.Request.Headers[HeaderNames.Authorization] != $"bearer {apiConfig.Value.MasterBearer}")
            context.Result = new UnauthorizedResult(); //This interestingly stops executing the action.
    }
}