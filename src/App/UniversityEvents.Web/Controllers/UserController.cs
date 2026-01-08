using Microsoft.AspNetCore.Mvc;

namespace UniversityEvents.Web.Controllers;

public class UserController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
