using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SwipetorApp.Areas.Admin.ViewModels;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Auth;
using SwipetorApp.Services.Contexts;
using SwipetorApp.System.UserRoleAuth;

namespace SwipetorApp.Areas.Admin;

[Area(AreaNames.Admin)]
[UserRoleAuth(UserRole.Admin)]
public class HomeController(IDbProvider dbProvider) : Controller
{
    // [Route("")]
    public IActionResult Index()
    {
        using var db = dbProvider.Create();
        var model = new AdminHomeViewModel
        {
            TotalUsers = db.Users.Count(),
            TotalPosts = db.Posts.Count()
        };


        return View(model);
    }
}