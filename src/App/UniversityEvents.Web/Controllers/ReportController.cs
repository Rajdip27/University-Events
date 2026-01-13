using Microsoft.AspNetCore.Mvc;

namespace UniversityEvents.Web.Controllers;

public class ReportController : Controller
{
    public IActionResult EventRegistrationReport()
    {
        return View();
    }
}
