using Microsoft.AspNetCore.Mvc;

namespace SwipetorApp.Controllers;

public class ErrorsController : Controller
{
    public IActionResult PageNotFound()
    {
        return View();
    }
}