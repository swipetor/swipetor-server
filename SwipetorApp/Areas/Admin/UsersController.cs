using Microsoft.AspNetCore.Mvc;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Auth;
using SwipetorApp.System.UserRoleAuth;

namespace SwipetorApp.Areas.Admin;

[Area(AreaNames.Admin)]
[UserRoleAuth(UserRole.Admin)]
public class UsersController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}