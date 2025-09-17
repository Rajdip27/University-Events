﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniversityEvents.Application.Logging;
using UniversityEvents.Application.Repositories.Auth;
using UniversityEvents.Application.ViewModel.Auth;
using static UniversityEvents.Core.Entities.Auth.IdentityModel;

namespace UniversityEvents.Web.Controllers.Auth
{
    public class AccountController(
        IExternalAuthService externalAuthService,
        SignInManager<User> signInManager,
        IAppLogger<AccountController> logger,IAuthService authService) : Controller
    {
        private readonly IExternalAuthService _externalAuthService = externalAuthService;
        private readonly IAuthService _authService = authService;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly IAppLogger<AccountController> _logger = logger;

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl= "/Dashboard")
        {
            _logger.LogInfo($"Login page accessed. ReturnUrl: {returnUrl}");
            return View(new LoginViewModel { ReturnUrl = returnUrl });
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
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = "/Dashboard")
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid registration model state.");
                return View(model);
            }

            _logger.LogInfo($"Registration attempt started for Email: {model.Email}");

            var result = await _authService.Register(model);

            if (!result.Success)
            {
                result.Errors.ForEach(e => ModelState.AddModelError("", e));
                _logger.LogWarning($"Registration failed for Email: {model.Email}. Errors: {string.Join(", ", result.Errors)}");
                return View(model);
            }

            var user = await _signInManager.UserManager.FindByIdAsync(result.UserId.ToString());
            if (user != null)
            {
                await _signInManager.SignInAsync(user, false);
                _logger.LogInfo($"User {user.Email} registered and logged in successfully.");
            }
            else
            {
                _logger.LogWarning($"User created but failed to log in. Email: {model.Email}");
            }

            return LocalRedirect(returnUrl);
        }



        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Login attempt failed due to invalid model state. Email: {model.Email}");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded)
            {
                _logger.LogInfo($"User {model.Email} logged in successfully.");
                return LocalRedirect(model.ReturnUrl ?? "/Dashboard");
            }

            _logger.LogWarning($"Invalid login attempt for email: {model.Email}");
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
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
                _logger.LogWarning("External login failed or user canceled the login process.");
                return RedirectToAction("Login");
            }

            _logger.LogInfo($"External login successful. Provider: {model.ReturnUrl}, Email: {model.Email}");
            return LocalRedirect(model.ReturnUrl ?? "/Dashboard");
        }
    }
}
