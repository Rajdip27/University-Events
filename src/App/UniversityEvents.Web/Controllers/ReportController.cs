using Microsoft.AspNetCore.Mvc;
using UniversityEvents.Application.Repositories;

namespace UniversityEvents.Web.Controllers;

public class ReportController(IEventRepository _eventRepository,IStudentRegistrationRepository studentRegistrationRepository) : Controller
{
    public async Task<IActionResult> EventRegistrationReport()
    {
        ViewData["Event"] = await _eventRepository.EventDropdown();
        ViewData["Student"] = await studentRegistrationRepository.StudentRegistrationDropdown();
        return View();
    }
}
