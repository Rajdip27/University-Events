using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniversityEvents.Application.Logging;
using UniversityEvents.Application.Repositories.Auth;
using UniversityEvents.Application.Services;
using UniversityEvents.Application.ViewModel.Auth;
using UniversityEvents.Application.ViewModel.ForgotPassword;
using static UniversityEvents.Core.Entities.Auth.IdentityModel;

namespace UniversityEvents.Web.Controllers.Auth;

public class AccountController(
    IExternalAuthService externalAuthService,
    SignInManager<User> signInManager,
    IAppLogger<AccountController> logger,
    IAuthService authService, IOtpService otpService, IResetPasswordService resetPasswordService) : Controller
{
    private readonly IExternalAuthService _externalAuthService = externalAuthService;
    private readonly IAuthService _authService = authService;
    private readonly SignInManager<User> _signInManager = signInManager;
    private readonly IAppLogger<AccountController> _logger = logger;
    private readonly IOtpService _otpService = otpService;
    private readonly IResetPasswordService _resetPasswordService = resetPasswordService;

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl)
    {
        _logger.LogInfo($"Login page accessed. ReturnUrl: {returnUrl}");
        return View(new LoginViewModel { ReturnUrl = returnUrl ?? "/" });
    }
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertMessage"] = "Invalid login details.";
            TempData["AlertType"] = "error";
            _logger.LogWarning($"Login attempt failed due to invalid model state. Email: {model.Email}");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

        if (result.Succeeded)
        {
            TempData["AlertMessage"] = "Login successful!";
            TempData["AlertType"] = "success";
            _logger.LogInfo($"User {model.Email} logged in successfully.");

            var user = await _signInManager.UserManager.FindByEmailAsync(model.Email);
            var roles = await _signInManager.UserManager.GetRolesAsync(user);

            if (roles.Contains("Administrator")) return LocalRedirect("/Dashboard");
            if (roles.Contains("Student")) return LocalRedirect("/Home");

            return LocalRedirect(model.ReturnUrl ?? "/");
        }

        TempData["AlertMessage"] = "Invalid email or password.";
        TempData["AlertType"] = "error";
        _logger.LogWarning($"Invalid login attempt for email: {model.Email}");
        return View(model);
    }
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }


    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertMessage"] = "Please fill all required fields correctly.";
            TempData["AlertType"] = "error";
            _logger.LogWarning("Invalid registration model state.");
            return View(model);
        }

        _logger.LogInfo($"Registration attempt started for Email: {model.Email}");
        var result = await _authService.Register(model);

        if (!result.Success)
        {
            result.Errors.ForEach(e => ModelState.AddModelError("", e));
            TempData["AlertMessage"] = string.Join(", ", result.Errors);
            TempData["AlertType"] = "error";
            _logger.LogWarning($"Registration failed for Email: {model.Email}. Errors: {string.Join(", ", result.Errors)}");
            return View(model);
        }

        var user = await _signInManager.UserManager.FindByIdAsync(result.UserId.ToString());
        if (user != null)
        {
            await _signInManager.SignInAsync(user, false);
            TempData["AlertMessage"] = "Registration successful!";
            TempData["AlertType"] = "success";
            await _authService.SendWelcomeEmail(model);
            _logger.LogInfo($"User {user.Email} registered and logged in successfully.");

            var roles = await _signInManager.UserManager.GetRolesAsync(user);
            if (roles.Contains("Administrator")) return LocalRedirect("/Dashboard");
            if (roles.Contains("Student")) return LocalRedirect("/Home");

            return LocalRedirect("/");
        }

        TempData["AlertMessage"] = "User created but failed to log in.";
        TempData["AlertType"] = "warning";
        _logger.LogWarning($"User created but failed to log in. Email: {model.Email}");
        return LocalRedirect("/");
    }
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLogin(string provider, string returnUrl)
    {
        _logger.LogInfo($"External login requested. Provider: {provider}, ReturnUrl: {returnUrl}");
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        var properties = await _externalAuthService.GetExternalLoginPropertiesAsync(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
    {
        _logger.LogInfo($"External login callback invoked. ReturnUrl: {returnUrl}");
        var model = await _externalAuthService.HandleExternalLoginAsync(returnUrl);

        if (model == null)
        {
            TempData["AlertMessage"] = "External login failed or canceled.";
            TempData["AlertType"] = "error";
            _logger.LogWarning("External login failed or user canceled the login process.");
            return RedirectToAction("Login");
        }

        var user = await _signInManager.UserManager.FindByEmailAsync(model.Email);
        if (user != null)
        {
            await _signInManager.SignInAsync(user, false);
            TempData["AlertMessage"] = "External login successful!";
            TempData["AlertType"] = "success";
            var roles = await _signInManager.UserManager.GetRolesAsync(user);
            if (roles.Contains("Administrator")) return LocalRedirect("/Dashboard");
            if (roles.Contains("Student")) return LocalRedirect("/Home");
            RegisterViewModel register= new RegisterViewModel
            {
                FullName = model.FullName,
                Email = model.Email
            };
            await _authService.SendWelcomeEmail(register);
            return LocalRedirect(model.ReturnUrl ?? "/");
        }

        TempData["AlertMessage"] = "External login succeeded but user not found.";
        TempData["AlertType"] = "warning";
        _logger.LogWarning($"External login succeeded but user not found in DB. Email: {model.Email}");
        return RedirectToAction("Login");
    }
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        TempData["AlertMessage"] = "Logged out successfully.";
        TempData["AlertType"] = "info";
        return RedirectToAction("Index", "Home");
    }
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    // POST: ForgotPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertMessage"] = "Please enter a valid email.";
            TempData["AlertType"] = "error";
            return View(model);
        }

        try
        {
            await _otpService.SendOtpAsync(model.Email);
            TempData["AlertMessage"] = "OTP sent successfully!";
            TempData["AlertType"] = "success";
            return RedirectToAction(nameof(VerifyOtp), new { email = model.Email });
        }
        catch (InvalidOperationException)
        {
            TempData["AlertMessage"] = "User not found!";
            TempData["AlertType"] = "error";
            return View(model);
        }
        catch (Exception ex)
        {
            TempData["AlertMessage"] = ex.Message;
            TempData["AlertType"] = "error";
            return View(model);
        }
    }
    [HttpGet]
    public IActionResult VerifyOtp(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            TempData["AlertMessage"] = "Email is required for OTP verification.";
            TempData["AlertType"] = "error";
            return RedirectToAction(nameof(ForgotPassword));
        }

        return View(new VerifyOtpDto { Email = email });
    }

    // POST: Verify OTP
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyOtp(VerifyOtpDto model)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertMessage"] = "Invalid OTP input.";
            TempData["AlertType"] = "error";
            return View(model);
        }

        var isValid = await _otpService.VerifyOtpAsync(model.Email, model.Otp);

        if (!isValid)
        {
            TempData["AlertMessage"] = "Invalid or expired OTP!";
            TempData["AlertType"] = "error";
            return View(model);
        }

        TempData["AlertMessage"] = "OTP verified successfully!";
        TempData["AlertType"] = "success";
        return RedirectToAction(nameof(ResetPassword), new { email = model.Email });
    }

    // GET: ResetPassword
    [HttpGet]
    public IActionResult ResetPassword(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            TempData["AlertMessage"] = "Email is required to reset password.";
            TempData["AlertType"] = "error";
            return RedirectToAction(nameof(ForgotPassword));
        }

        return View(new ResetPasswordDto { Email = email });
    }

    // POST: ResetPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertMessage"] = "Please provide a valid password.";
            TempData["AlertType"] = "error";
            return View(model);
        }

        try
        {
            await _resetPasswordService.ResetPasswordAsync(model.Email, model.NewPassword);
            TempData["AlertMessage"] = "Password reset successfully!";
            TempData["AlertType"] = "success";
            return RedirectToAction("Login", "Account");
        }
        catch (Exception ex)
        {
            TempData["AlertMessage"] = ex.Message;
            TempData["AlertType"] = "error";
            return View(model);
        }
    }
    [HttpPost]
    public async Task<IActionResult> ResendOtp([FromForm] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            TempData["AlertMessage"] = "Email is required.";
            TempData["AlertType"] = "error";
            return RedirectToAction("VerifyOtp");
        }

        try
        {
            await _otpService.SendOtpAsync(email);
            TempData["AlertMessage"] = "A new OTP has been sent to your email.";
            TempData["AlertType"] = "success";
        }
        catch (Exception ex)
        {
            TempData["AlertMessage"] = ex.Message;
            TempData["AlertType"] = "error";
        }

        return RedirectToAction("VerifyOtp", new { email });
    }

    
}
