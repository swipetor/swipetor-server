using Microsoft.AspNetCore.Mvc;

namespace SwipetorApp.Controllers;

public class PagesController : Controller
{
    public IActionResult Terms()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
}