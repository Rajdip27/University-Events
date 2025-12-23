using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.Logging;
using UniversityEvents.Application.Repositories;
using UniversityEvents.Application.ViewModel;

namespace UniversityEvents.Web.Controllers;

[Route("StudentRegistration")]
public class StudentRegistrationController(
    IEventRepository eventRepository,
    IStudentRegistrationRepository studentRegistration,
    IAppLogger<StudentRegistrationController> logger) : Controller
{
    [HttpGet("Register/{slug}/{referrerId}")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(string slug, int referrerId)
    {
        logger.LogInfo($"GET Register called | Slug: {slug}, ReferrerId: {referrerId}");

        if (string.IsNullOrEmpty(slug))
        {
            logger.LogWarning("Register called with empty slug");
            return NotFound();
        }

        var studentRegistrationVm = new StudentRegistrationVm
        {
            Event = new EventVm()
        };

        if (referrerId > 0)
        {
            var data = await eventRepository.GetByIdAsync(referrerId);
            if (data != null)
            {
                studentRegistrationVm.Event = data;
            }
            else
            {
                logger.LogWarning($"Event not found | ReferrerId: {referrerId}");
            }
        }

        if (!User.Identity.IsAuthenticated)
        {
            logger.LogInfo("Unauthenticated user redirected to Login");

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
        logger.LogInfo("POST Register called");

        if (!ModelState.IsValid)
        {
            logger.LogWarning("Register validation failed");
            return View(model);
        }

        try
        {
            var result = await studentRegistration.CreateOrUpdateRegistrationAsync(model, CancellationToken.None);

            if (result != null)
            {
                logger.LogInfo($"Student registered successfully | RegistrationId: {result.Id}");
                return RedirectToAction("RegisterSuccess", new { id = result.Id });
            }

            logger.LogWarning("Registration failed: result is null");
            return View(model);
        }
        catch (Exception ex)
        {
            logger.LogError("Error occurred while registering student", ex);
            throw;
        }
    }
    [HttpGet("RegisterSuccess/{id}")]
    public async Task<IActionResult> RegisterSuccess(long id)
    {
        logger.LogInfo($"RegisterSuccess called | Id: {id}");

        var registration = await studentRegistration.GetRegistrationByIdAsync(id, CancellationToken.None);

        if (registration == null)
        {
            logger.LogWarning($"Registration not found | Id: {id}");
            return NotFound();
        }

        return View(registration);
    }
    [HttpGet]
    public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10)
    {
        logger.LogInfo($"Index called | Search: {search}, Page: {page}, PageSize: {pageSize}");
        try
        {
            var filter = new Filter
            {
                Search = search,
                IsDelete = false,
                Page = page,
                PageSize = pageSize
            };

            var registrations = await studentRegistration.GetRegistrationsAsync(filter, CancellationToken.None);
            return View(registrations);
        }
        catch (Exception ex)
        {
            logger.LogError("Error occurred while loading registration list", ex);
            throw;
        }
    }
}
