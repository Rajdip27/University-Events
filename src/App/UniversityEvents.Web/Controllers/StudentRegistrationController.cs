using Microsoft.AspNetCore.Mvc;

namespace UniversityEvents.Web.Controllers;

public class StudentRegistrationController : Controller
{
    [HttpGet("/StudentRegistration/Register/{slug}/{referrerId?}")]
    public async Task<IActionResult> Register(string slug, int? referrerId)
    {
        if (string.IsNullOrEmpty(slug))
            return NotFound();
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account", new
            {
                ReturnUrl = Url.Action("Register", new { slug, referrerId })
            });
        }
        return View();
    }
}
