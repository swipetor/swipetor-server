using System;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwipetorApp.Models.Enums;
using SwipetorApp.Models.ViewModels;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.Users;
using WebLibServer.Photos;

namespace SwipetorApp.Controllers;

public class FallbackController : Controller
{
    public IActionResult Index()
    {
        var requestPath = Request.Path.ToString();

        // Check if this is an API request and return 404 for those
        if (requestPath.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            return NotFound();
        
        //redirect 301
        return RedirectPermanent("/");
    }
}
