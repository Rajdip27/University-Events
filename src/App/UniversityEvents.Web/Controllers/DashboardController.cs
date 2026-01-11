using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UniversityEvents.Application.Repositories;

namespace UniversityEvents.Web.Controllers;
[Authorize]
[Route("Dashboard")]
public class DashboardController(IDashboardRepository dashboardRepository) : Controller
{
    [HttpGet]
    public IActionResult Index(long? eventId)
    {
        var model = dashboardRepository.GetDashboardData(eventId);
        return View(model); 
    }
}
