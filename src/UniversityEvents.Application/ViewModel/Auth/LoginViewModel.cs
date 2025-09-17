using System.ComponentModel.DataAnnotations;

namespace UniversityEvents.Application.ViewModel.Auth;

public class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, DataType(DataType.Password)]
    public string Password { get; set; }

    public string ReturnUrl { get; set; } = "/Dashboard";
}
