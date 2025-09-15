using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using UniversityEvents.Application.ViewModel.Auth;
using static UniversityEvents.Core.Entities.Auth.IdentityModel;
using System.Linq;

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

            try
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null) return null;
                // Try signing in directly
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

                // Extract user info from external provider
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);

                // Check if user already exists
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
                    {
                        return null;
                    }
                }

                // Add external login if not already added
                var userLogins = await _userManager.GetLoginsAsync(user);
                if (!userLogins.Any(l => l.LoginProvider == info.LoginProvider && l.ProviderKey == info.ProviderKey))
                {
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    if (!addLoginResult.Succeeded)
                    {
                        return null;
                    }
                }

                // Assign "User" role if not already assigned
                if (!await _userManager.IsInRoleAsync(user, "User"))
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }
                // Sign in user
                await _signInManager.SignInAsync(user, isPersistent: false);

                return new ExternalLoginViewModel
                {
                    Email = email,
                    Provider = info.LoginProvider,
                    ReturnUrl = returnUrl
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            
        }
    }
}
