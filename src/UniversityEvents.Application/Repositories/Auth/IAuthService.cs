using Microsoft.AspNetCore.Identity;
using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Services;
using UniversityEvents.Application.ViewModel.Auth;
using static UniversityEvents.Core.Entities.Auth.IdentityModel;

namespace UniversityEvents.Application.Repositories.Auth;

public interface IAuthService
{
    Task<RegistrationResponse> Register(RegisterViewModel model);
    Task SendWelcomeEmail(RegisterViewModel model);

}

public class AuthService( UserManager<User> _userManager,IEmailService emailService,IRazorViewToStringRenderer razorViewToStringRenderer) : IAuthService
{


 

    public async Task<RegistrationResponse> Register(RegisterViewModel request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new RegistrationResponse
            {
                Success = false,
                Errors = new() { $"Email '{request.Email}' is already registered." }
            };
        }

        var user = new User
        {
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber,
            Name = request.FullName,
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return new RegistrationResponse
            {
                Success = false,
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        await _userManager.AddToRoleAsync(user, "Student");

        return new RegistrationResponse
        {
            Success = true,
            UserId = user.Id
        };
    }

    public async Task SendWelcomeEmail(RegisterViewModel result)
    {
        if (result == null)
            return;
        var htmlContent =
            await razorViewToStringRenderer
                .RenderViewToStringAsync(
                    "EmailTemplates/WelcomeEmailTemplates",
                    result
                );
        var emailMessage = new EmailMessage
        {
            To = new List<string> { result.Email },
            Subject = "Welcome to EventHub! 🎉",
            HtmlFilePath = htmlContent   // HTML string
        };

        await emailService.SendEmailAsync(emailMessage);
    }
}


