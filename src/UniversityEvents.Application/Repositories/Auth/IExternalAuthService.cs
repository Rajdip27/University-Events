using System.Security.Claims;
using System.Security.Policy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniversityEvents.Application.Mappers;
using UniversityEvents.Application.ViewModel.Auth;

namespace UniversityEvents.Application.Repositories.Auth;

public interface IExternalAuthService
{
    Task<AuthenticationProperties> GetExternalLoginPropertiesAsync(string provider, string redirectUrl);
    Task<ExternalLoginViewModel> HandleExternalLoginAsync(string returnUrl = null);
}

public class ExternalAuthService : IExternalAuthService
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public ExternalAuthService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public Task<AuthenticationProperties> GetExternalLoginPropertiesAsync(string provider, string redirectUrl)
        => Task.FromResult(_signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl));

    public async Task<ExternalLoginViewModel> HandleExternalLoginAsync(string returnUrl = null)
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null) return null;

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        if (result.Succeeded)
            return ExternalLoginMapper.MapToViewModel(info, returnUrl);

        var email = info.Principal.FindFirstValue(System.Security.Claims.ClaimTypes.Email);
        var user = new IdentityUser { UserName = email, Email = email };
        var createResult = await _userManager.CreateAsync(user);
        if (createResult.Succeeded)
        {
            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return ExternalLoginMapper.MapToViewModel(info, returnUrl);
        }

        return null;
    }
}




 //private readonly IExternalAuthService _externalAuthService;

 //   public AccountController(IExternalAuthService externalAuthService)
 //   {
 //       _externalAuthService = externalAuthService;
 //   }

 //   [HttpPost]
 //   [AllowAnonymous]
 //   public async Task<IActionResult> ExternalLogin(string provider, string returnUrl = null)
 //   {
 //       var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
 //       var properties = await _externalAuthService.GetExternalLoginPropertiesAsync(provider, redirectUrl);
 //       return Challenge(properties, provider);
 //   }

 //   [AllowAnonymous]
 //   public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
 //   {
 //       var model = await _externalAuthService.HandleExternalLoginAsync(returnUrl);
 //       if (model == null) return RedirectToAction("Login");

 //       return LocalRedirect(model.ReturnUrl ?? "/");
 //   }

 //   [AllowAnonymous]
 //   public IActionResult Login() => View();
