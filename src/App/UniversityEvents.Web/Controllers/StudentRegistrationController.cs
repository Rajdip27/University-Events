using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.Logging;
using UniversityEvents.Application.Repositories;
using UniversityEvents.Application.ViewModel;
using UniversityEvents.Infrastructure.Healper.Acls;

namespace UniversityEvents.Web.Controllers;

[Route("StudentRegistration")]
public class StudentRegistrationController(
    IEventRepository eventRepository,
    IStudentRegistrationRepository studentRegistration,
    IAppLogger<StudentRegistrationController> logger, ISignInHelper signInHelper, IPaymentRepository paymentRepository) : Controller
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

        if (!signInHelper.UserId.HasValue)
        {
            TempData["AlertMessage"] = "User is not logged in.";
            TempData["AlertType"] = "Error";
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var existingRegistration =
                await studentRegistration.GetStudentRegistrationAsync(
                    model.EventId,
                    signInHelper.UserId.Value,
                    CancellationToken.None);

            // 🔴 Already Registered
            if (existingRegistration != null)
            {
                TempData["AlertMessage"] = "You have already been registered for this event.";
                TempData["AlertType"] = "Warning";

                logger.LogWarning("Registration attempt failed: already registered");


                return RedirectToAction("AlreadyApplied", new { eventId = existingRegistration.Id });
            }

            // 🟢 New Registration
            var result =
                await studentRegistration.CreateOrUpdateRegistrationAsync(
                    model,
                    CancellationToken.None);

            if (result == null)
            {
                TempData["AlertMessage"] = "Registration failed. Please try again.";
                TempData["AlertType"] = "Error";
                return View(model);
            }

            logger.LogInfo($"Student registered successfully | RegistrationId: {result.Id}");
            return RedirectToAction("RegisterSuccess", new { id = result.Id });
        }
        catch (Exception ex)
        {
            logger.LogError("Error occurred while registering student", ex);

            TempData["AlertMessage"] = "An unexpected error occurred. Please try again later.";
            TempData["AlertType"] = "Error";

            return View(model);
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
            if (signInHelper.Roles.Contains(AppRoles.Student)) // put correct role name
            {
                filter.UserId = signInHelper.UserId ??0;
            }
            var registrations = await studentRegistration.GetRegistrationsAsync(filter, CancellationToken.None);
            return View(registrations);
        }
        catch (Exception ex)
        {
            logger.LogError("Error occurred while loading registration list", ex);
            throw;
        }
    }

    [HttpGet("AlreadyApplied/{eventId}")]
    public async Task<IActionResult> AlreadyApplied(long eventId)
    {
        var registration = await studentRegistration.GetRegistrationByIdAsync(eventId, CancellationToken.None);
        if (registration == null)
            return NotFound();
        return View(registration);
    }
    [HttpGet("PaidInvoice")]
    public async Task<IActionResult> PaidInvoice(long registerId)
    {
        var data= await paymentRepository.GetByIdAsync(registerId, CancellationToken.None);
        return View(data);
    }

}
