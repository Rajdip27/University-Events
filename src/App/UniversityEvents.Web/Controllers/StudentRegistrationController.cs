using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using UniversityEvents.Application.Repositories;
using UniversityEvents.Application.ViewModel;

namespace UniversityEvents.Web.Controllers;

[Route("StudentRegistration")]
public class StudentRegistrationController(IEventRepository eventRepository, IStudentRegistrationRepository studentRegistration) : Controller
{
    [HttpGet("Register/{slug}/{referrerId}")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(string slug, int referrerId)
    {
        if (string.IsNullOrEmpty(slug))
            return NotFound();

        var studentRegistrationVm = new StudentRegistrationVm
        {
            Event = new EventVm() // initialize to avoid null
        };

        if (referrerId > 0)
        {
            var data = await eventRepository.GetByIdAsync((long)referrerId);
            if (data != null)
            {
                studentRegistrationVm.Event = data;
            }
        }
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account", new
            {
                ReturnUrl = Url.Action("Register", "StudentRegistration", new { slug, referrerId })
            });
        }
        return View(studentRegistrationVm);
    }


    [HttpPost("Register/{slug}/{referrerId?}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(StudentRegistrationVm model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var result = await studentRegistration.CreateOrUpdateRegistrationAsync(model, CancellationToken.None);
        if (result is not null)
        {
            return RedirectToAction(
    "RegisterSuccess",
    "StudentRegistration",
    new { id = result.Id }
);

        }
        return View(model);
    }
    [HttpGet("RegisterSuccess/{id}")]
    public async Task<IActionResult> RegisterSuccess(long id)
    {
        StudentRegistrationVm studentRegistrationVm = new StudentRegistrationVm();
        var registration = await studentRegistration.GetRegistrationByIdAsync(id, CancellationToken.None);
        if (studentRegistrationVm == null) return NotFound();
        return View(studentRegistrationVm);
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View();
    }

}
