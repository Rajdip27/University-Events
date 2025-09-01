using Microsoft.AspNetCore.Mvc;

namespace UniversityEvents.Web.Controllers;

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
