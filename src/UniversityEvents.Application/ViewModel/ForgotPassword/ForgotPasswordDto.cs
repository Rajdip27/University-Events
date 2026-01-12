using System.ComponentModel.DataAnnotations;

namespace UniversityEvents.Application.ViewModel.ForgotPassword;

public class ForgotPasswordDto
{
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; } = null!;
}
