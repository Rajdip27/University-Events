using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using UniversityEvents.Application.ViewModel.Auth;
using static UniversityEvents.Core.Entities.Auth.IdentityModel;

namespace UniversityEvents.Application.Repositories.Auth
{
    public interface IExternalAuthService
    {
        Task<AuthenticationProperties> GetExternalLoginPropertiesAsync(string provider, string redirectUrl);
        Task<ExternalLoginViewModel> HandleExternalLoginAsync(string returnUrl = null);
    }

    public class ExternalAuthService : IExternalAuthService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public ExternalAuthService(SignInManager<User> signInManager, UserManager<User> userManager)
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

            // Try signing in directly with external login
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (signInResult.Succeeded)
            {
                return new ExternalLoginViewModel
                {
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                    Provider = info.LoginProvider,
                    ReturnUrl = returnUrl
                };
            }

            // Extract user info
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
                return null; // Cannot create user without email

            // Check if user exists
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Create new user
                user = new User
                {
                    UserName = email,
                    Email = email,
                    Name = name,
                    CreatedDate = DateTimeOffset.UtcNow
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    return null;
            }

            // Assign "User" role if not already assigned
            if (!await _userManager.IsInRoleAsync(user, "Student"))
                await _userManager.AddToRoleAsync(user, "Student");

            // Sign in user — this automatically links the external login if it isn’t already linked
            await _signInManager.SignInAsync(user, isPersistent: false);

            return new ExternalLoginViewModel
            {
                Email = email,
                Provider = info.LoginProvider,
                ReturnUrl = returnUrl
            };
        }

    }
}
