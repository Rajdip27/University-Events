using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityEvents.Application.ViewModel;

namespace UniversityEvents.Web.Controllers;

public class StudentRegistrationController(IEventRepository eventRepository) : Controller
{
    // GET
    [Route("StudentRegistration")]
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

    // POST
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(StudentRegistrationVm model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> RegisterSuccess(long id)
    {
        StudentRegistrationVm studentRegistrationVm = new StudentRegistrationVm();
        //var registration = await _registrationService.GetLatestRegistrationForEventAsync(id);
        if (studentRegistrationVm == null) return NotFound();
        return View(studentRegistrationVm);

    }


    }
